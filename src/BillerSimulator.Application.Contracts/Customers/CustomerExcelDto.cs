using System;

namespace BillerSimulator.Customers
{
    public class CustomerExcelDto
    {
        public string? CustomerNumber { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
    }
}