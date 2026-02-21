using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Radzen;
using Disnegativos.Shared.DTOs;
using Disnegativos.Shared.Interfaces;
using Disnegativos.Shared.Services.Interfaces;
using Disnegativos.Shared.Data.Entities;
using Microsoft.AspNetCore.SignalR.Client;

namespace Disnegativos.Shared.Pages.Organizations;

public partial class OrganizationsPage : ComponentBase, IAsyncDisposable
{
    [Inject] private IOrganizationService OrganizationService { get; set; } = default!;
    [Inject] private NotificationService NotificationService { get; set; } = default!;
    [Inject] private DialogService DialogService { get; set; } = default!;
    [Inject] private HubConnection HubConnection { get; set; } = default!;
    [Inject] private IServiceProvider ServiceProvider { get; set; } = default!;

    private List<OrganizationDto> _organizations = [];
    private List<SportDiscipline> _disciplines = [];
    private List<CountryDto> _countries = [];
    private bool _isLoading = true;
    private bool _isEditing = false;
    private OrganizationEditDto _editingOrganization = new();
    private ISyncService? _syncService;

    protected override async Task OnInitializedAsync()
    {
        _syncService = ServiceProvider.GetService<ISyncService>();

        await LoadDataAsync();

        HubConnection.On<string, Guid, string>("EntityChanged", async (entityType, entityId, changeType) =>
        {
            if (entityType == "Organization")
            {
                if (_syncService != null)
                {
                    await _syncService.PullServerChangesAsync();
                }

                await LoadOrganizationsAsync();
                await InvokeAsync(StateHasChanged);

                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Info,
                    Summary = L["DataUpdated"],
                    Detail = L["ClubUpdatedByAnotherUser"]
                });
            }
        });

        if (HubConnection.State == HubConnectionState.Disconnected)
        {
            try { await HubConnection.StartAsync(); }
            catch { /* Offline */ }
        }
    }

    private async Task LoadDataAsync()
    {
        _isLoading = true;
        await Task.WhenAll(LoadOrganizationsAsync(), LoadAuxiliaryDataAsync());
        _isLoading = false;
    }

    private async Task LoadOrganizationsAsync()
    {
        _organizations = await OrganizationService.GetAllOrganizationsAsync();
    }

    private async Task LoadAuxiliaryDataAsync()
    {
        _disciplines = await OrganizationService.GetDisciplinesAsync();
        _countries = await OrganizationService.GetCountriesAsync();
    }

    private void HandleAdd()
    {
        _editingOrganization = new OrganizationEditDto { Id = Guid.NewGuid() };
        _isEditing = true;
    }

    private async Task HandleEditAsync(Guid id)
    {
        var org = await OrganizationService.GetOrganizationForEditAsync(id);
        if (org != null)
        {
            _editingOrganization = org;
            _isEditing = true;
        }
    }

    private async Task HandleSaveAsync(OrganizationEditDto model)
    {
        await OrganizationService.SaveOrganizationAsync(model);

        if (_syncService != null)
        {
            await _syncService.PushPendingChangesAsync();
        }

        if (HubConnection.State == HubConnectionState.Connected)
        {
            await HubConnection.InvokeAsync("NotifyEntityChanged", "Organization", model.Id, "Modified");
        }

        _isEditing = false;
        await LoadOrganizationsAsync();

        NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = L["Success"], Detail = L["SavedSuccessfully"] });
    }

    private void HandleCancel()
    {
        _isEditing = false;
        _editingOrganization = new();
    }

    private async Task HandleDeleteAsync(Guid id)
    {
        var confirm = await DialogService.Confirm(L["AreYouSure"], L["Delete"], new ConfirmOptions { OkButtonText = L["Yes"], CancelButtonText = L["No"] });

        if (confirm == true)
        {
            await OrganizationService.DeleteOrganizationAsync(id);

            if (_syncService != null)
            {
                await _syncService.PushPendingChangesAsync();
            }

            if (HubConnection.State == HubConnectionState.Connected)
            {
                await HubConnection.InvokeAsync("NotifyEntityChanged", "Organization", id, "Deleted");
            }

            await LoadOrganizationsAsync();
            NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = L["Success"], Detail = L["DeletedSuccessfully"] });
        }
    }

    public async ValueTask DisposeAsync()
    {
        HubConnection.Remove("EntityChanged");
        await ValueTask.CompletedTask;
    }
}
