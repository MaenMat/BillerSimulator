using BillerSimulator.Invoices;
using System;

namespace BillerSimulator.Invoices
{
    public class InvoiceExcelDto
    {
        public string? InvoiceNumber { get; set; }
        public double Amount { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime PaidDate { get; set; }
        public InvoiceStatus InvoiceStatus { get; set; }
    }
}