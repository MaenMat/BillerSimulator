using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using BillerSimulator.Customers;
using Volo.Abp.Content;
using BillerSimulator.Shared;

namespace BillerSimulator.Controllers.Customers
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Customer")]
    [Route("api/app/customers")]

    public class CustomerController : AbpController, ICustomersAppService
    {
        private readonly ICustomersAppService _customersAppService;

        public CustomerController(ICustomersAppService customersAppService)
        {
            _customersAppService = customersAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<CustomerDto>> GetListAsync(GetCustomersInput input)
        {
            return _customersAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<CustomerDto> GetAsync(Guid id)
        {
            return _customersAppService.GetAsync(id);
        }

        [HttpPost]
        public virtual Task<CustomerDto> CreateAsync(CustomerCreateDto input)
        {
            return _customersAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<CustomerDto> UpdateAsync(Guid id, CustomerUpdateDto input)
        {
            return _customersAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _customersAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(CustomerExcelDownloadDto input)
        {
            return _customersAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _customersAppService.GetDownloadTokenAsync();
        }
    }
}