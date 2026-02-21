using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Radzen;
using Disnegativos.Shared.DTOs;
using Disnegativos.Shared.Interfaces;
using Disnegativos.Shared.Services.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;

namespace Disnegativos.Shared.Pages.Countries;

public partial class CountriesPage : ComponentBase, IAsyncDisposable
{
    // IServiceProvider para resolver ISyncService opcionalmente (solo existe en MAUI)
    [Inject] private IServiceProvider ServiceProvider { get; set; } = default!;

    private List<CountryDto> _countries = [];
    private bool _isLoading = true;
    private bool _isEditing = false;
    private CountryEditDto _editingCountry = new();

    // ISyncService es null en Web (no registrado), presente en MAUI
    private ISyncService? _syncService;

    protected override async Task OnInitializedAsync()
    {
        // Resolver opcionalmente el SyncService (solo MAUI lo tiene registrado)
        _syncService = ServiceProvider.GetService<ISyncService>();

        await LoadCountriesAsync();

        // ── SignalR: Recibir cambios en tiempo real ──
        HubConnection.On<string, Guid, string>("EntityChanged", async (entityType, entityId, changeType) =>
        {
            if (entityType == "Country")
            {
                // En MAUI: hacer Pull para bajar los datos reales del servidor a la BD local
                if (_syncService != null)
                {
                    await _syncService.PullServerChangesAsync();
                }

                // Recargar la lista local (en Web ya tiene los datos, en MAUI acaba de recibirlos)
                await LoadCountriesAsync();
                await InvokeAsync(StateHasChanged);

                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Info,
                    Summary = L["DataUpdated"],
                    Detail = L["CountryUpdatedByAnotherUser"]
                });
            }
        });

        if (HubConnection.State == HubConnectionState.Disconnected)
        {
            try { await HubConnection.StartAsync(); }
            catch { /* Offline */ }
        }
    }

    private async Task LoadCountriesAsync()
    {
        _isLoading = true;
        _countries = await CountryService.GetAllCountriesAsync();
        _isLoading = false;
    }

    private void HandleAdd()
    {
        _editingCountry = new CountryEditDto();
        _isEditing = true;
    }

    private async Task HandleEditAsync(Guid id)
    {
        var country = await CountryService.GetCountryForEditAsync(id);
        if (country != null)
        {
            _editingCountry = country;
            _isEditing = true;
        }
    }

    private async Task HandleSaveAsync(CountryEditDto model)
    {
        await CountryService.SaveCountryAsync(model);

        // MAUI: push inmediato al servidor sin esperar al ciclo de sync periódico
        if (_syncService != null)
        {
            await _syncService.PushPendingChangesAsync();
        }

        // Notificar vía SignalR a los demás clientes
        if (HubConnection.State == HubConnectionState.Connected)
        {
            await HubConnection.InvokeAsync("NotifyEntityChanged", "Country", model.Id, "Modified");
        }

        _isEditing = false;
        await LoadCountriesAsync();

        NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = L["Success"], Detail = L["SavedSuccessfully"] });
    }

    private void HandleCancel()
    {
        _isEditing = false;
        _editingCountry = new();
    }

    private async Task HandleDeleteAsync(Guid id)
    {
        var confirm = await DialogService.Confirm(L["AreYouSure"], L["Delete"], new ConfirmOptions { OkButtonText = L["Yes"], CancelButtonText = L["No"] });

        if (confirm == true)
        {
            await CountryService.DeleteCountryAsync(id);

            // MAUI: push inmediato
            if (_syncService != null)
            {
                await _syncService.PushPendingChangesAsync();
            }

            // Notificar vía SignalR
            if (HubConnection.State == HubConnectionState.Connected)
            {
                await HubConnection.InvokeAsync("NotifyEntityChanged", "Country", id, "Deleted");
            }

            await LoadCountriesAsync();
            NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = L["Success"], Detail = L["DeletedSuccessfully"] });
        }
    }

    public async ValueTask DisposeAsync()
    {
        HubConnection.Remove("EntityChanged");
        await ValueTask.CompletedTask;
    }
}
