using BillerSimulator.Shared;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using BillerSimulator.Shared;

namespace BillerSimulator.Invoices
{
    public interface IInvoicesAppService : IApplicationService
    {
        Task<PagedResultDto<InvoiceWithNavigationPropertiesDto>> GetListAsync(GetInvoicesInput input);

        Task<InvoiceWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<InvoiceDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetCustomerLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<InvoiceDto> CreateAsync(InvoiceCreateDto input);

        Task<InvoiceDto> UpdateAsync(Guid id, InvoiceUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(InvoiceExcelDownloadDto input);

        Task<DownloadTokenResultDto> GetDownloadTokenAsync();

        Task<object> GetTheAmountForSpecificInvoice(string invoiceNumber);

        Task<object> GetTheAmountForSpecificCustomer(string customerNumber);

        Task<bool> PayTheInvoiceAsync(string invoiceNumber);
    }
}