using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace BillerSimulator.Customers
{
    public class CustomerUpdateDto : IHasConcurrencyStamp
    {
        public string? CustomerNumber { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? Address { get; set; }

        public string ConcurrencyStamp { get; set; }
    }
}