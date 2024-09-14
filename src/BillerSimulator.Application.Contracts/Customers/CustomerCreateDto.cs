using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BillerSimulator.Customers
{
    public class CustomerCreateDto
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? Address { get; set; }
    }
}