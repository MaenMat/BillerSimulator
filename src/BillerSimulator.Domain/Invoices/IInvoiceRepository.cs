using BillerSimulator.Invoices;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace BillerSimulator.Invoices
{
    public interface IInvoiceRepository : IRepository<Invoice, Guid>
    {
        Task<InvoiceWithNavigationProperties> GetWithNavigationPropertiesAsync(
    Guid id,
    CancellationToken cancellationToken = default
);

        Task<List<InvoiceWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
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
            CancellationToken cancellationToken = default
        );

        Task<List<Invoice>> GetListAsync(
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
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
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
            CancellationToken cancellationToken = default);
    }
}