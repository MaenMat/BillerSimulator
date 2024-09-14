using Volo.Abp.Application.Dtos;
using System;

namespace BillerSimulator.Customers
{
    public class GetCustomersInput : PagedAndSortedResultRequestDto
    {
        public string? FilterText { get; set; }

        public string? CustomerNumber { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }

        public GetCustomersInput()
        {

        }
    }
}