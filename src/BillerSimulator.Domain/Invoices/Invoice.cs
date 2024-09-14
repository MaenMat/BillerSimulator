using BillerSimulator.Invoices;
using BillerSimulator.Customers;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace BillerSimulator.Invoices
{
    public class Invoice : FullAuditedAggregateRoot<Guid>
    {
        [CanBeNull]
        public virtual string? InvoiceNumber { get; set; }

        public virtual double Amount { get; set; }

        public virtual DateTime DueDate { get; set; }

        public virtual DateTime PaidDate { get; set; }

        public virtual InvoiceStatus InvoiceStatus { get; set; }
        public Guid CustomerId { get; set; }

        public Invoice()
        {

        }

        public Invoice(Guid id, Guid customerId, string invoiceNumber, double amount, DateTime dueDate, DateTime paidDate, InvoiceStatus invoiceStatus)
        {

            Id = id;
            InvoiceNumber = invoiceNumber;
            Amount = amount;
            DueDate = dueDate;
            PaidDate = paidDate;
            InvoiceStatus = invoiceStatus;
            CustomerId = customerId;
        }

    }
}