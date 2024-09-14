using BillerSimulator.Customers;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace BillerSimulator.Invoices
{
    public class InvoiceWithNavigationPropertiesDto
    {
        public InvoiceDto Invoice { get; set; }

        public CustomerDto Customer { get; set; }

    }
}