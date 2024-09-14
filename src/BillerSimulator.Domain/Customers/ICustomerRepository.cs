using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace BillerSimulator.Customers
{
    public interface ICustomerRepository : IRepository<Customer, Guid>
    {
        Task<List<Customer>> GetListAsync(
            string filterText = null,
            string customerNumber = null,
            string fullName = null,
            string phoneNumber = null,
            string email = null,
            string address = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<long> GetCountAsync(
            string filterText = null,
            string customerNumber = null,
            string fullName = null,
            string phoneNumber = null,
            string email = null,
            string address = null,
            CancellationToken cancellationToken = default);
    }
}