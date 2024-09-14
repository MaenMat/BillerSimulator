using BillerSimulator.Invoices;
using Volo.Abp.Application.Dtos;
using System;

namespace BillerSimulator.Invoices
{
    public class InvoiceExcelDownloadDto
    {
        public string DownloadToken { get; set; }

        public string? FilterText { get; set; }

        public string? InvoiceNumber { get; set; }
        public double? AmountMin { get; set; }
        public double? AmountMax { get; set; }
        public DateTime? DueDateMin { get; set; }
        public DateTime? DueDateMax { get; set; }
        public DateTime? PaidDateMin { get; set; }
        public DateTime? PaidDateMax { get; set; }
        public InvoiceStatus? InvoiceStatus { get; set; }
        public Guid? CustomerId { get; set; }

        public InvoiceExcelDownloadDto()
        {

        }
    }
}