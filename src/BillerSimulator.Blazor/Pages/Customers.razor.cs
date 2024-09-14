using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.DataGrid;
using Volo.Abp.BlazoriseUI.Components;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using BillerSimulator.Customers;
using BillerSimulator.Permissions;
using BillerSimulator.Shared;

namespace BillerSimulator.Blazor.Pages
{
    public partial class Customers
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private IReadOnlyList<CustomerDto> CustomerList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateCustomer { get; set; }
        private bool CanEditCustomer { get; set; }
        private bool CanDeleteCustomer { get; set; }
        private CustomerCreateDto NewCustomer { get; set; }
        private Validations NewCustomerValidations { get; set; } = new();
        private CustomerUpdateDto EditingCustomer { get; set; }
        private Validations EditingCustomerValidations { get; set; } = new();
        private Guid EditingCustomerId { get; set; }
        private Modal CreateCustomerModal { get; set; } = new();
        private Modal EditCustomerModal { get; set; } = new();
        private GetCustomersInput Filter { get; set; }
        private DataGridEntityActionsColumn<CustomerDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "customer-create-tab";
        protected string SelectedEditTab = "customer-edit-tab";
        
        public Customers()
        {
            NewCustomer = new CustomerCreateDto();
            EditingCustomer = new CustomerUpdateDto();
            Filter = new GetCustomersInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            CustomerList = new List<CustomerDto>();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Customers"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewCustomer"], async () =>
            {
                await OpenCreateCustomerModalAsync();
            }, IconName.Add, requiredPolicyName: BillerSimulatorPermissions.Customers.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateCustomer = await AuthorizationService
                .IsGrantedAsync(BillerSimulatorPermissions.Customers.Create);
            CanEditCustomer = await AuthorizationService
                            .IsGrantedAsync(BillerSimulatorPermissions.Customers.Edit);
            CanDeleteCustomer = await AuthorizationService
                            .IsGrantedAsync(BillerSimulatorPermissions.Customers.Delete);
        }

        private async Task GetCustomersAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await CustomersAppService.GetListAsync(Filter);
            CustomerList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetCustomersAsync();
            await InvokeAsync(StateHasChanged);
        }

        private  async Task DownloadAsExcelAsync()
        {
            var token = (await CustomersAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("BillerSimulator") ??
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/customers/as-excel-file?DownloadToken={token}&FilterText={Filter.FilterText}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<CustomerDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetCustomersAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateCustomerModalAsync()
        {
            NewCustomer = new CustomerCreateDto{
                
                
            };
            await NewCustomerValidations.ClearAll();
            await CreateCustomerModal.Show();
        }

        private async Task CloseCreateCustomerModalAsync()
        {
            NewCustomer = new CustomerCreateDto{
                
                
            };
            await CreateCustomerModal.Hide();
        }

        private async Task OpenEditCustomerModalAsync(CustomerDto input)
        {
            var customer = await CustomersAppService.GetAsync(input.Id);
            
            EditingCustomerId = customer.Id;
            EditingCustomer = ObjectMapper.Map<CustomerDto, CustomerUpdateDto>(customer);
            await EditingCustomerValidations.ClearAll();
            await EditCustomerModal.Show();
        }

        private async Task DeleteCustomerAsync(CustomerDto input)
        {
            await CustomersAppService.DeleteAsync(input.Id);
            await GetCustomersAsync();
        }

        private async Task CreateCustomerAsync()
        {
            try
            {
                if (await NewCustomerValidations.ValidateAll() == false)
                {
                    return;
                }

                await CustomersAppService.CreateAsync(NewCustomer);
                await GetCustomersAsync();
                await CloseCreateCustomerModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditCustomerModalAsync()
        {
            await EditCustomerModal.Hide();
        }

        private async Task UpdateCustomerAsync()
        {
            try
            {
                if (await EditingCustomerValidations.ValidateAll() == false)
                {
                    return;
                }

                await CustomersAppService.UpdateAsync(EditingCustomerId, EditingCustomer);
                await GetCustomersAsync();
                await EditCustomerModal.Hide();                
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private void OnSelectedCreateTabChanged(string name)
        {
            SelectedCreateTab = name;
        }

        private void OnSelectedEditTabChanged(string name)
        {
            SelectedEditTab = name;
        }
        

    }
}
