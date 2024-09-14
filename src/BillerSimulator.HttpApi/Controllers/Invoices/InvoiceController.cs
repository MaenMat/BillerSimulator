using BillerSimulator.Shared;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using BillerSimulator.Invoices;
using Volo.Abp.Content;
using BillerSimulator.Shared;

namespace BillerSimulator.Controllers.Invoices
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Invoice")]
    [Route("api/app/invoices")]

    public class InvoiceController : AbpController, IInvoicesAppService
    {
        private readonly IInvoicesAppService _invoicesAppService;

        public InvoiceController(IInvoicesAppService invoicesAppService)
        {
            _invoicesAppService = invoicesAppService;
        }

        [HttpGet]
        public Task<PagedResultDto<InvoiceWithNavigationPropertiesDto>> GetListAsync(GetInvoicesInput input)
        {
            return _invoicesAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public Task<InvoiceWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return _invoicesAppService.GetWithNavigationPropertiesAsync(id);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<InvoiceDto> GetAsync(Guid id)
        {
            return _invoicesAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("customer-lookup")]
        public Task<PagedResultDto<LookupDto<Guid>>> GetCustomerLookupAsync(LookupRequestDto input)
        {
            return _invoicesAppService.GetCustomerLookupAsync(input);
        }

        [HttpPost]
        public virtual Task<InvoiceDto> CreateAsync(InvoiceCreateDto input)
        {
            return _invoicesAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<InvoiceDto> UpdateAsync(Guid id, InvoiceUpdateDto input)
        {
            return _invoicesAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _invoicesAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(InvoiceExcelDownloadDto input)
        {
            return _invoicesAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _invoicesAppService.GetDownloadTokenAsync();
        }
        [HttpGet]
        [Route("get-invoice-amount")]
        public Task<object> GetTheAmountForSpecificInvoice(string invoiceNumber)
        {
            return _invoicesAppService.GetTheAmountForSpecificInvoice(invoiceNumber);
        }
        [HttpGet]
        [Route("get-customer-amount")]
        public Task<object> GetTheAmountForSpecificCustomer(string customerNumber)
        {
            return _invoicesAppService.GetTheAmountForSpecificCustomer(customerNumber);
        }
        [HttpPost]
        [Route("pay-the-invoice")]
        public Task<bool> PayTheInvoiceAsync(string invoiceNumber)
        {
            return _invoicesAppService.PayTheInvoiceAsync(invoiceNumber);
        }
    }
}