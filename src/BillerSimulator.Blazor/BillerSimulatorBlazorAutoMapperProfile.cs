using BillerSimulator.Invoices;
using Volo.Abp.AutoMapper;
using BillerSimulator.Customers;
using AutoMapper;

namespace BillerSimulator.Blazor;

public class BillerSimulatorBlazorAutoMapperProfile : Profile
{
    public BillerSimulatorBlazorAutoMapperProfile()
    {
        //Define your AutoMapper configuration here for the Blazor project.

        CreateMap<CustomerDto, CustomerUpdateDto>();

        CreateMap<InvoiceDto, InvoiceUpdateDto>();
    }
}