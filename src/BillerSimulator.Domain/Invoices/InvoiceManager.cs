using BillerSimulator.Invoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace BillerSimulator.Invoices
{
    public class InvoiceManager : DomainService
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public InvoiceManager(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<Invoice> CreateAsync(
        Guid customerId, double amount, DateTime dueDate, DateTime paidDate, InvoiceStatus invoiceStatus)
        {
            Check.NotNull(customerId, nameof(customerId));
            Check.NotNull(dueDate, nameof(dueDate));
            Check.NotNull(paidDate, nameof(paidDate));
            Check.NotNull(invoiceStatus, nameof(invoiceStatus));
            var invoiecNumber = await GenerateUniqueInvoiceNumberAsync();

            var invoice = new Invoice(
             GuidGenerator.Create(),
             customerId, invoiecNumber, amount, dueDate, paidDate, invoiceStatus
             );

            return await _invoiceRepository.InsertAsync(invoice);
        }

        public async Task<Invoice> UpdateAsync(
            Guid id,
            Guid customerId, string invoiceNumber, double amount, DateTime dueDate, DateTime paidDate, InvoiceStatus invoiceStatus, [CanBeNull] string concurrencyStamp = null
        )
        {
            Check.NotNull(customerId, nameof(customerId));
            Check.NotNull(dueDate, nameof(dueDate));
            Check.NotNull(paidDate, nameof(paidDate));
            Check.NotNull(invoiceStatus, nameof(invoiceStatus));

            var invoice = await _invoiceRepository.GetAsync(id);

            invoice.CustomerId = customerId;
            invoice.InvoiceNumber = invoiceNumber;
            invoice.Amount = amount;
            invoice.DueDate = dueDate;
            invoice.PaidDate = paidDate;
            invoice.InvoiceStatus = invoiceStatus;

            invoice.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _invoiceRepository.UpdateAsync(invoice);
        }
        public async Task<string> GenerateUniqueInvoiceNumberAsync()
        {
            Random random = new Random();
            string invoiceNumber;
            bool isUnique;
            do
            {

                int uniqueNumber = random.Next(10000000, 99999999);
                invoiceNumber = uniqueNumber.ToString();
                isUnique = await _invoiceRepository.FirstOrDefaultAsync(x => x.InvoiceNumber == invoiceNumber) == null;

            } while (!isUnique);

            return invoiceNumber;
        }

    }
}