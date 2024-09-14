using BillerSimulator.Invoices;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BillerSimulator.Invoices
{
    public class InvoiceCreateDto
    {
        public double Amount { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime PaidDate { get; set; }
        public InvoiceStatus InvoiceStatus { get; set; } = ((InvoiceStatus[])Enum.GetValues(typeof(InvoiceStatus)))[0];
        public Guid CustomerId { get; set; }
    }
}