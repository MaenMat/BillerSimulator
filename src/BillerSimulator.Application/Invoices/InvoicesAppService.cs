using BillerSimulator.Shared;
using BillerSimulator.Customers;
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
using BillerSimulator.Invoices;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using BillerSimulator.Shared;
using Volo.Abp.Uow;

namespace BillerSimulator.Invoices
{
    [RemoteService(IsEnabled = false)]
    [Authorize(BillerSimulatorPermissions.Invoices.Default)]
    public class InvoicesAppService : ApplicationService, IInvoicesAppService
    {
        private readonly IDistributedCache<InvoiceExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly InvoiceManager _invoiceManager;
        private readonly IRepository<Customer, Guid> _customerRepository;

        public InvoicesAppService(IInvoiceRepository invoiceRepository, InvoiceManager invoiceManager, IDistributedCache<InvoiceExcelDownloadTokenCacheItem, string> excelDownloadTokenCache, IRepository<Customer, Guid> customerRepository)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _invoiceRepository = invoiceRepository;
            _invoiceManager = invoiceManager; _customerRepository = customerRepository;
        }

        public virtual async Task<PagedResultDto<InvoiceWithNavigationPropertiesDto>> GetListAsync(GetInvoicesInput input)
        {
            var totalCount = await _invoiceRepository.GetCountAsync(input.FilterText, input.InvoiceNumber, input.AmountMin, input.AmountMax, input.DueDateMin, input.DueDateMax, input.PaidDateMin, input.PaidDateMax, input.InvoiceStatus, input.CustomerId);
            var items = await _invoiceRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.InvoiceNumber, input.AmountMin, input.AmountMax, input.DueDateMin, input.DueDateMax, input.PaidDateMin, input.PaidDateMax, input.InvoiceStatus, input.CustomerId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<InvoiceWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<InvoiceWithNavigationProperties>, List<InvoiceWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<InvoiceWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<InvoiceWithNavigationProperties, InvoiceWithNavigationPropertiesDto>
                (await _invoiceRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<InvoiceDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Invoice, InvoiceDto>(await _invoiceRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetCustomerLookupAsync(LookupRequestDto input)
        {
            var query = (await _customerRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.FullName != null &&
                         x.FullName.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Customer>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Customer>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        [Authorize(BillerSimulatorPermissions.Invoices.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _invoiceRepository.DeleteAsync(id);
        }

        [Authorize(BillerSimulatorPermissions.Invoices.Create)]
        public virtual async Task<InvoiceDto> CreateAsync(InvoiceCreateDto input)
        {
            if (input.CustomerId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["Customer"]]);
            }

            var invoice = await _invoiceManager.CreateAsync(
            input.CustomerId, input.Amount, input.DueDate, input.PaidDate, input.InvoiceStatus
            );

            return ObjectMapper.Map<Invoice, InvoiceDto>(invoice);
        }

        [Authorize(BillerSimulatorPermissions.Invoices.Edit)]
        public virtual async Task<InvoiceDto> UpdateAsync(Guid id, InvoiceUpdateDto input)
        {
            if (input.CustomerId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["Customer"]]);
            }

            var invoice = await _invoiceManager.UpdateAsync(
            id,
            input.CustomerId, input.InvoiceNumber, input.Amount, input.DueDate, input.PaidDate, input.InvoiceStatus, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<Invoice, InvoiceDto>(invoice);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(InvoiceExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var invoices = await _invoiceRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.InvoiceNumber, input.AmountMin, input.AmountMax, input.DueDateMin, input.DueDateMax, input.PaidDateMin, input.PaidDateMax, input.InvoiceStatus);
            var items = invoices.Select(item => new
            {
                InvoiceNumber = item.Invoice.InvoiceNumber,
                Amount = item.Invoice.Amount,
                DueDate = item.Invoice.DueDate,
                PaidDate = item.Invoice.PaidDate,
                InvoiceStatus = item.Invoice.InvoiceStatus,

                Customer = item.Customer?.FullName,

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "Invoices.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new InvoiceExcelDownloadTokenCacheItem { Token = token },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });

            return new DownloadTokenResultDto
            {
                Token = token
            };
        }

        public async Task<object> GetTheAmountForSpecificInvoice(string invoiceNumber)
        {
            var invoiceQuery = await _invoiceRepository.GetQueryableAsync();
            var invoice = invoiceQuery.Where(i => i.InvoiceNumber == invoiceNumber).FirstOrDefault();
            if (invoice == null)
            {
                throw new Exception(L["NoInvoiceWithThisNumber"]);
            }
            var customer = await _customerRepository.GetAsync(invoice.CustomerId);
            return new
            {
                InvoiceNumber = invoice.InvoiceNumber,
                CustomerName = customer.FullName,
                Amount = invoice.Amount
            };
        }

        public async Task<object> GetTheAmountForSpecificCustomer(string customerNumber)
        {
            var customer = await _customerRepository.FirstOrDefaultAsync(c => c.CustomerNumber == customerNumber);

            if (customer == null)
            {
                throw new UserFriendlyException("Customer not found");
            }
            var invoiceQuery = await _invoiceRepository.GetQueryableAsync();
            var invoices = invoiceQuery
                .Where(i => i.CustomerId == customer.Id).ToList();

            var totalAmount = invoices.Sum(i => i.Amount);

            return new
            {
                CustomerName = customer.FullName,
                Amount = totalAmount,
            };
        }
        [UnitOfWork]
        public async Task<bool> PayTheInvoiceAsync(string invoiceNumber)
        {
            var invoiceQuery = await _invoiceRepository.GetQueryableAsync();
            var invoice = invoiceQuery.Where(i => i.InvoiceNumber == invoiceNumber).FirstOrDefault();
            if (invoice == null)
            {
                throw new Exception(L["NoInvoiceWithThisNumber"]);
            }
            invoice.InvoiceStatus = InvoiceStatus.Paid;
            await _invoiceRepository.UpdateAsync(invoice);
            return true;
        }
    }
}