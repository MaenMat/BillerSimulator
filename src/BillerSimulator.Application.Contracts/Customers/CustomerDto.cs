using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace BillerSimulator.Customers
{
    public class CustomerDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string? CustomerNumber { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }

        public string ConcurrencyStamp { get; set; }
    }
}