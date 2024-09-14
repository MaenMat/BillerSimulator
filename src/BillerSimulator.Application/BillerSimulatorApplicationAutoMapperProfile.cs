using BillerSimulator.Invoices;
using System;
using BillerSimulator.Shared;
using Volo.Abp.AutoMapper;
using BillerSimulator.Customers;
using AutoMapper;

namespace BillerSimulator;

public class BillerSimulatorApplicationAutoMapperProfile : Profile
{
    public BillerSimulatorApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        CreateMap<Customer, CustomerDto>();
        CreateMap<Customer, CustomerExcelDto>();

        CreateMap<Invoice, InvoiceDto>();
        CreateMap<Invoice, InvoiceExcelDto>();
        CreateMap<InvoiceWithNavigationProperties, InvoiceWithNavigationPropertiesDto>();
        CreateMap<Customer, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.FullName));
    }
}