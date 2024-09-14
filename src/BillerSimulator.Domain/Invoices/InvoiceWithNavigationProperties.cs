using BillerSimulator.Customers;

using System;
using System.Collections.Generic;

namespace BillerSimulator.Invoices
{
    public class InvoiceWithNavigationProperties
    {
        public Invoice Invoice { get; set; }

        public Customer Customer { get; set; }
        

        
    }
}