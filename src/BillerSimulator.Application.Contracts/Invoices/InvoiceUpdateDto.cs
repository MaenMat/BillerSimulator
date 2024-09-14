using BillerSimulator.Invoices;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace BillerSimulator.Invoices
{
    public class InvoiceUpdateDto : IHasConcurrencyStamp
    {
        public string? InvoiceNumber { get; set; }
        public double Amount { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime PaidDate { get; set; }
        public InvoiceStatus InvoiceStatus { get; set; }
        public Guid CustomerId { get; set; }

        public string ConcurrencyStamp { get; set; }
    }
}