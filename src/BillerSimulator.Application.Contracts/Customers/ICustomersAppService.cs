using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using BillerSimulator.Shared;

namespace BillerSimulator.Customers
{
    public interface ICustomersAppService : IApplicationService
    {
        Task<PagedResultDto<CustomerDto>> GetListAsync(GetCustomersInput input);

        Task<CustomerDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<CustomerDto> CreateAsync(CustomerCreateDto input);

        Task<CustomerDto> UpdateAsync(Guid id, CustomerUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(CustomerExcelDownloadDto input);

        Task<DownloadTokenResultDto> GetDownloadTokenAsync();
    }
}