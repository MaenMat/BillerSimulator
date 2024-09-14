using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using BillerSimulator.EntityFrameworkCore;

namespace BillerSimulator.Customers
{
    public class EfCoreCustomerRepository : EfCoreRepository<BillerSimulatorDbContext, Customer, Guid>, ICustomerRepository
    {
        public EfCoreCustomerRepository(IDbContextProvider<BillerSimulatorDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public async Task<List<Customer>> GetListAsync(
            string filterText = null,
            string customerNumber = null,
            string fullName = null,
            string phoneNumber = null,
            string email = null,
            string address = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, customerNumber, fullName, phoneNumber, email, address);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? CustomerConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public async Task<long> GetCountAsync(
            string filterText = null,
            string customerNumber = null,
            string fullName = null,
            string phoneNumber = null,
            string email = null,
            string address = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetDbSetAsync()), filterText, customerNumber, fullName, phoneNumber, email, address);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Customer> ApplyFilter(
            IQueryable<Customer> query,
            string filterText,
            string customerNumber = null,
            string fullName = null,
            string phoneNumber = null,
            string email = null,
            string address = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.CustomerNumber.Contains(filterText) || e.FullName.Contains(filterText) || e.PhoneNumber.Contains(filterText) || e.Email.Contains(filterText) || e.Address.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(customerNumber), e => e.CustomerNumber.Contains(customerNumber))
                    .WhereIf(!string.IsNullOrWhiteSpace(fullName), e => e.FullName.Contains(fullName))
                    .WhereIf(!string.IsNullOrWhiteSpace(phoneNumber), e => e.PhoneNumber.Contains(phoneNumber))
                    .WhereIf(!string.IsNullOrWhiteSpace(email), e => e.Email.Contains(email))
                    .WhereIf(!string.IsNullOrWhiteSpace(address), e => e.Address.Contains(address));
        }
    }
}