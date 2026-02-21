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
using Microsoft.AspNetCore.SignalR.Client;

namespace Disnegativos.Shared.Pages.Referees;

public partial class RefereesPage : ComponentBase, IAsyncDisposable
{
    [Inject] private IRefereeService RefereeService { get; set; } = default!;
    [Inject] private ICountryService CountryService { get; set; } = default!;
    [Inject] private NotificationService NotificationService { get; set; } = default!;
    [Inject] private DialogService DialogService { get; set; } = default!;
    [Inject] private HubConnection HubConnection { get; set; } = default!;
    [Inject] private IServiceProvider ServiceProvider { get; set; } = default!;

    private List<RefereeDto> _referees = [];
    private List<CountryDto> _countries = [];
    private bool _isLoading = true;
    private bool _isEditing = false;
    private RefereeEditDto _editingReferee = new();
    private ISyncService? _syncService;

    protected override async Task OnInitializedAsync()
    {
        _syncService = ServiceProvider.GetService<ISyncService>();

        await LoadDataAsync();

        HubConnection.On<string, Guid, string>("EntityChanged", async (entityType, entityId, changeType) =>
        {
            if (entityType == "Referee")
            {
                if (_syncService != null)
                {
                    await _syncService.PullServerChangesAsync();
                }

                await LoadRefereesAsync();
                await InvokeAsync(StateHasChanged);

                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Info,
                    Summary = L["DataUpdated"],
                    Detail = L["RefereeUpdatedByAnotherUser"]
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
        await Task.WhenAll(LoadRefereesAsync(), LoadCountriesAsync());
        _isLoading = false;
    }

    private async Task LoadRefereesAsync()
    {
        _referees = await RefereeService.GetAllRefereesAsync();
    }

    private async Task LoadCountriesAsync()
    {
        _countries = await CountryService.GetAllCountriesAsync();
    }

    private void HandleAdd()
    {
        _editingReferee = new RefereeEditDto { Id = Guid.NewGuid() };
        _isEditing = true;
    }

    private async Task HandleEditAsync(Guid id)
    {
        var referee = await RefereeService.GetRefereeForEditAsync(id);
        if (referee != null)
        {
            _editingReferee = referee;
            _isEditing = true;
        }
    }

    private async Task HandleSaveAsync(RefereeEditDto model)
    {
        await RefereeService.SaveRefereeAsync(model);

        if (_syncService != null)
        {
            await _syncService.PushPendingChangesAsync();
        }

        if (HubConnection.State == HubConnectionState.Connected)
        {
            await HubConnection.InvokeAsync("NotifyEntityChanged", "Referee", model.Id, "Modified");
        }

        _isEditing = false;
        await LoadRefereesAsync();

        NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = L["Success"], Detail = L["SavedSuccessfully"] });
    }

    private void HandleCancel()
    {
        _isEditing = false;
        _editingReferee = new();
    }

    private async Task HandleDeleteAsync(Guid id)
    {
        var confirm = await DialogService.Confirm(L["AreYouSure"], L["Delete"], new ConfirmOptions { OkButtonText = L["Yes"], CancelButtonText = L["No"] });

        if (confirm == true)
        {
            await RefereeService.DeleteRefereeAsync(id);

            if (_syncService != null)
            {
                await _syncService.PushPendingChangesAsync();
            }

            if (HubConnection.State == HubConnectionState.Connected)
            {
                await HubConnection.InvokeAsync("NotifyEntityChanged", "Referee", id, "Deleted");
            }

            await LoadRefereesAsync();
            NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = L["Success"], Detail = L["DeletedSuccessfully"] });
        }
    }

    public async ValueTask DisposeAsync()
    {
        HubConnection.Remove("EntityChanged");
        await ValueTask.CompletedTask;
    }
}
