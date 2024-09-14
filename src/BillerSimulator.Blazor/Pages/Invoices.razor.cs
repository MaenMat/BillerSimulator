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
using BillerSimulator.Invoices;
using BillerSimulator.Permissions;
using BillerSimulator.Shared;

namespace BillerSimulator.Blazor.Pages
{
    public partial class Invoices
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private IReadOnlyList<InvoiceWithNavigationPropertiesDto> InvoiceList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateInvoice { get; set; }
        private bool CanEditInvoice { get; set; }
        private bool CanDeleteInvoice { get; set; }
        private InvoiceCreateDto NewInvoice { get; set; }
        private Validations NewInvoiceValidations { get; set; } = new();
        private InvoiceUpdateDto EditingInvoice { get; set; }
        private Validations EditingInvoiceValidations { get; set; } = new();
        private Guid EditingInvoiceId { get; set; }
        private Modal CreateInvoiceModal { get; set; } = new();
        private Modal EditInvoiceModal { get; set; } = new();
        private GetInvoicesInput Filter { get; set; }
        private DataGridEntityActionsColumn<InvoiceWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "invoice-create-tab";
        protected string SelectedEditTab = "invoice-edit-tab";
        private IReadOnlyList<LookupDto<Guid>> CustomersCollection { get; set; } = new List<LookupDto<Guid>>();

        public Invoices()
        {
            NewInvoice = new InvoiceCreateDto();
            EditingInvoice = new InvoiceUpdateDto();
            Filter = new GetInvoicesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            InvoiceList = new List<InvoiceWithNavigationPropertiesDto>();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
            await GetCustomerCollectionLookupAsync();


        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Invoices"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewInvoice"], async () =>
            {
                await OpenCreateInvoiceModalAsync();
            }, IconName.Add, requiredPolicyName: BillerSimulatorPermissions.Invoices.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateInvoice = await AuthorizationService
                .IsGrantedAsync(BillerSimulatorPermissions.Invoices.Create);
            CanEditInvoice = await AuthorizationService
                            .IsGrantedAsync(BillerSimulatorPermissions.Invoices.Edit);
            CanDeleteInvoice = await AuthorizationService
                            .IsGrantedAsync(BillerSimulatorPermissions.Invoices.Delete);
        }

        private async Task GetInvoicesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await InvoicesAppService.GetListAsync(Filter);
            InvoiceList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetInvoicesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private  async Task DownloadAsExcelAsync()
        {
            var token = (await InvoicesAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("BillerSimulator") ??
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/invoices/as-excel-file?DownloadToken={token}&FilterText={Filter.FilterText}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<InvoiceWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetInvoicesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateInvoiceModalAsync()
        {
            NewInvoice = new InvoiceCreateDto{
                DueDate = DateTime.Now,
PaidDate = DateTime.Now,

                CustomerId = CustomersCollection.Select(i=>i.Id).FirstOrDefault(),

            };
            await NewInvoiceValidations.ClearAll();
            await CreateInvoiceModal.Show();
        }

        private async Task CloseCreateInvoiceModalAsync()
        {
            NewInvoice = new InvoiceCreateDto{
                DueDate = DateTime.Now,
PaidDate = DateTime.Now,

                CustomerId = CustomersCollection.Select(i=>i.Id).FirstOrDefault(),

            };
            await CreateInvoiceModal.Hide();
        }

        private async Task OpenEditInvoiceModalAsync(InvoiceWithNavigationPropertiesDto input)
        {
            var invoice = await InvoicesAppService.GetWithNavigationPropertiesAsync(input.Invoice.Id);
            
            EditingInvoiceId = invoice.Invoice.Id;
            EditingInvoice = ObjectMapper.Map<InvoiceDto, InvoiceUpdateDto>(invoice.Invoice);
            await EditingInvoiceValidations.ClearAll();
            await EditInvoiceModal.Show();
        }

        private async Task DeleteInvoiceAsync(InvoiceWithNavigationPropertiesDto input)
        {
            await InvoicesAppService.DeleteAsync(input.Invoice.Id);
            await GetInvoicesAsync();
        }

        private async Task CreateInvoiceAsync()
        {
            try
            {
                if (await NewInvoiceValidations.ValidateAll() == false)
                {
                    return;
                }

                await InvoicesAppService.CreateAsync(NewInvoice);
                await GetInvoicesAsync();
                await CloseCreateInvoiceModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditInvoiceModalAsync()
        {
            await EditInvoiceModal.Hide();
        }

        private async Task UpdateInvoiceAsync()
        {
            try
            {
                if (await EditingInvoiceValidations.ValidateAll() == false)
                {
                    return;
                }

                await InvoicesAppService.UpdateAsync(EditingInvoiceId, EditingInvoice);
                await GetInvoicesAsync();
                await EditInvoiceModal.Hide();                
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
        

        private async Task GetCustomerCollectionLookupAsync(string? newValue = null)
        {
            CustomersCollection = (await InvoicesAppService.GetCustomerLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

    }
}
