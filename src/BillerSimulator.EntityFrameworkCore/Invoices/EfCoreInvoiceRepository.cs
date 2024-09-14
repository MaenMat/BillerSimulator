using BillerSimulator.Invoices;
using BillerSimulator.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using BillerSimulator.EntityFrameworkCore;

namespace BillerSimulator.Invoices
{
    public class EfCoreInvoiceRepository : EfCoreRepository<BillerSimulatorDbContext, Invoice, Guid>, IInvoiceRepository
    {
        public EfCoreInvoiceRepository(IDbContextProvider<BillerSimulatorDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public async Task<InvoiceWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
                .Select(invoice => new InvoiceWithNavigationProperties
                {
                    Invoice = invoice,
                    Customer = dbContext.Set<Customer>().FirstOrDefault(c => c.Id == invoice.CustomerId)
                }).FirstOrDefault();
        }

        public async Task<List<InvoiceWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            string invoiceNumber = null,
            double? amountMin = null,
            double? amountMax = null,
            DateTime? dueDateMin = null,
            DateTime? dueDateMax = null,
            DateTime? paidDateMin = null,
            DateTime? paidDateMax = null,
            InvoiceStatus? invoiceStatus = null,
            Guid? customerId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, invoiceNumber, amountMin, amountMax, dueDateMin, dueDateMax, paidDateMin, paidDateMax, invoiceStatus, customerId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? InvoiceConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        protected virtual async Task<IQueryable<InvoiceWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from invoice in (await GetDbSetAsync())
                   join customer in (await GetDbContextAsync()).Set<Customer>() on invoice.CustomerId equals customer.Id into customers
                   from customer in customers.DefaultIfEmpty()
                   select new InvoiceWithNavigationProperties
                   {
                       Invoice = invoice,
                       Customer = customer
                   };
        }

        protected virtual IQueryable<InvoiceWithNavigationProperties> ApplyFilter(
            IQueryable<InvoiceWithNavigationProperties> query,
            string filterText,
            string invoiceNumber = null,
            double? amountMin = null,
            double? amountMax = null,
            DateTime? dueDateMin = null,
            DateTime? dueDateMax = null,
            DateTime? paidDateMin = null,
            DateTime? paidDateMax = null,
            InvoiceStatus? invoiceStatus = null,
            Guid? customerId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Invoice.InvoiceNumber.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(invoiceNumber), e => e.Invoice.InvoiceNumber.Contains(invoiceNumber))
                    .WhereIf(amountMin.HasValue, e => e.Invoice.Amount >= amountMin.Value)
                    .WhereIf(amountMax.HasValue, e => e.Invoice.Amount <= amountMax.Value)
                    .WhereIf(dueDateMin.HasValue, e => e.Invoice.DueDate >= dueDateMin.Value)
                    .WhereIf(dueDateMax.HasValue, e => e.Invoice.DueDate <= dueDateMax.Value)
                    .WhereIf(paidDateMin.HasValue, e => e.Invoice.PaidDate >= paidDateMin.Value)
                    .WhereIf(paidDateMax.HasValue, e => e.Invoice.PaidDate <= paidDateMax.Value)
                    .WhereIf(invoiceStatus.HasValue, e => e.Invoice.InvoiceStatus == invoiceStatus)
                    .WhereIf(customerId != null && customerId != Guid.Empty, e => e.Customer != null && e.Customer.Id == customerId);
        }

        public async Task<List<Invoice>> GetListAsync(
            string filterText = null,
            string invoiceNumber = null,
            double? amountMin = null,
            double? amountMax = null,
            DateTime? dueDateMin = null,
            DateTime? dueDateMax = null,
            DateTime? paidDateMin = null,
            DateTime? paidDateMax = null,
            InvoiceStatus? invoiceStatus = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, invoiceNumber, amountMin, amountMax, dueDateMin, dueDateMax, paidDateMin, paidDateMax, invoiceStatus);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? InvoiceConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public async Task<long> GetCountAsync(
            string filterText = null,
            string invoiceNumber = null,
            double? amountMin = null,
            double? amountMax = null,
            DateTime? dueDateMin = null,
            DateTime? dueDateMax = null,
            DateTime? paidDateMin = null,
            DateTime? paidDateMax = null,
            InvoiceStatus? invoiceStatus = null,
            Guid? customerId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, invoiceNumber, amountMin, amountMax, dueDateMin, dueDateMax, paidDateMin, paidDateMax, invoiceStatus, customerId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Invoice> ApplyFilter(
            IQueryable<Invoice> query,
            string filterText,
            string invoiceNumber = null,
            double? amountMin = null,
            double? amountMax = null,
            DateTime? dueDateMin = null,
            DateTime? dueDateMax = null,
            DateTime? paidDateMin = null,
            DateTime? paidDateMax = null,
            InvoiceStatus? invoiceStatus = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.InvoiceNumber.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(invoiceNumber), e => e.InvoiceNumber.Contains(invoiceNumber))
                    .WhereIf(amountMin.HasValue, e => e.Amount >= amountMin.Value)
                    .WhereIf(amountMax.HasValue, e => e.Amount <= amountMax.Value)
                    .WhereIf(dueDateMin.HasValue, e => e.DueDate >= dueDateMin.Value)
                    .WhereIf(dueDateMax.HasValue, e => e.DueDate <= dueDateMax.Value)
                    .WhereIf(paidDateMin.HasValue, e => e.PaidDate >= paidDateMin.Value)
                    .WhereIf(paidDateMax.HasValue, e => e.PaidDate <= paidDateMax.Value)
                    .WhereIf(invoiceStatus.HasValue, e => e.InvoiceStatus == invoiceStatus);
        }
    }
}