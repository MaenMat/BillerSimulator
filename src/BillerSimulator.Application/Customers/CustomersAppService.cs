using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using BillerSimulator.Permissions;
using BillerSimulator.Customers;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using BillerSimulator.Shared;
using BillerSimulator.Invoices;

namespace BillerSimulator.Customers
{
    [RemoteService(IsEnabled = false)]
    [Authorize(BillerSimulatorPermissions.Customers.Default)]
    public class CustomersAppService : ApplicationService, ICustomersAppService
    {
        private readonly IDistributedCache<CustomerExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        private readonly ICustomerRepository _customerRepository;
        private readonly CustomerManager _customerManager;
        private readonly IInvoiceRepository _invoiceRepository;
        public CustomersAppService(ICustomerRepository customerRepository, CustomerManager customerManager, IInvoiceRepository invoiceRepository, IDistributedCache<CustomerExcelDownloadTokenCacheItem, string> excelDownloadTokenCache)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _customerRepository = customerRepository;
            _customerManager = customerManager;
            _invoiceRepository = invoiceRepository; 
        }

        public virtual async Task<PagedResultDto<CustomerDto>> GetListAsync(GetCustomersInput input)
        {
            var totalCount = await _customerRepository.GetCountAsync(input.FilterText, input.CustomerNumber, input.FullName, input.PhoneNumber, input.Email, input.Address);
            var items = await _customerRepository.GetListAsync(input.FilterText, input.CustomerNumber, input.FullName, input.PhoneNumber, input.Email, input.Address, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<CustomerDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Customer>, List<CustomerDto>>(items)
            };
        }

        public virtual async Task<CustomerDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Customer, CustomerDto>(await _customerRepository.GetAsync(id));
        }

        [Authorize(BillerSimulatorPermissions.Customers.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            if(await _invoiceRepository.AnyAsync(i => i.CustomerId == id))
            {
                throw new UserFriendlyException(L["CantDeleteCustomerLinkedWithInvoices"]);
            }
            await _customerRepository.DeleteAsync(id);
        }

        [Authorize(BillerSimulatorPermissions.Customers.Create)]
        public virtual async Task<CustomerDto> CreateAsync(CustomerCreateDto input)
        {

            var customer = await _customerManager.CreateAsync(input.FullName, input.PhoneNumber, input.Email, input.Address
            );

            return ObjectMapper.Map<Customer, CustomerDto>(customer);
        }

        [Authorize(BillerSimulatorPermissions.Customers.Edit)]
        public virtual async Task<CustomerDto> UpdateAsync(Guid id, CustomerUpdateDto input)
        {

            var customer = await _customerManager.UpdateAsync(
            id,
            input.CustomerNumber, input.FullName, input.PhoneNumber, input.Email, input.Address, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<Customer, CustomerDto>(customer);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(CustomerExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _customerRepository.GetListAsync(input.FilterText, input.CustomerNumber, input.FullName, input.PhoneNumber, input.Email, input.Address);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<Customer>, List<CustomerExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "Customers.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new CustomerExcelDownloadTokenCacheItem { Token = token },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });

            return new DownloadTokenResultDto
            {
                Token = token
            };
        }
    }
}