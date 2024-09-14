using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace BillerSimulator.Customers
{
    public class CustomerManager : DomainService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerManager(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<Customer> CreateAsync( 
            string fullName, string phoneNumber, string email, string address)
        {
            Check.NotNullOrWhiteSpace(fullName, nameof(fullName));
            Check.NotNullOrWhiteSpace(phoneNumber, nameof(phoneNumber));
            var customerNumber = await GenerateUniqueCustomerNumberAsync();

            var customer = new Customer(
             GuidGenerator.Create(),
             customerNumber, fullName, phoneNumber, email, address
             );

            return await _customerRepository.InsertAsync(customer);
        }

        public async Task<Customer> UpdateAsync(
            Guid id,
            string customerNumber, string fullName, string phoneNumber, string email, string address, [CanBeNull] string concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(fullName, nameof(fullName));
            Check.NotNullOrWhiteSpace(phoneNumber, nameof(phoneNumber));

            var customer = await _customerRepository.GetAsync(id);

            customer.CustomerNumber = customerNumber;
            customer.FullName = fullName;
            customer.PhoneNumber = phoneNumber;
            customer.Email = email;
            customer.Address = address;

            customer.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _customerRepository.UpdateAsync(customer);
        }
        public async Task<string> GenerateUniqueCustomerNumberAsync()
        {
            Random random = new Random();
            string customerNumber;
            bool isUnique;
            do
            {
                int uniqueNumber = random.Next(10000000, 99999999);
                customerNumber = uniqueNumber.ToString();
                isUnique = await _customerRepository.FirstOrDefaultAsync(x => x.CustomerNumber == customerNumber) == null;

            } while (!isUnique);

            return customerNumber;
        }

    }
}