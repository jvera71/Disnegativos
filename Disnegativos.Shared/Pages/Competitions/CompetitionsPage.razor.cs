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

namespace Disnegativos.Shared.Pages.Competitions;

public partial class CompetitionsPage : ComponentBase, IAsyncDisposable
{
    [Inject] private ICompetitionService CompetitionService { get; set; } = default!;
    [Inject] private NotificationService NotificationService { get; set; } = default!;
    [Inject] private DialogService DialogService { get; set; } = default!;
    [Inject] private HubConnection HubConnection { get; set; } = default!;
    [Inject] private IServiceProvider ServiceProvider { get; set; } = default!;

    private List<CompetitionDto> _competitions = [];
    private List<SportDiscipline> _disciplines = [];
    private List<SportCategory> _categories = [];
    private bool _isLoading = true;
    private bool _isEditing = false;
    private CompetitionEditDto _editingCompetition = new();
    private ISyncService? _syncService;

    protected override async Task OnInitializedAsync()
    {
        _syncService = ServiceProvider.GetService<ISyncService>();

        await LoadDataAsync();

        HubConnection.On<string, Guid, string>("EntityChanged", async (entityType, entityId, changeType) =>
        {
            if (entityType == "Competition")
            {
                if (_syncService != null)
                {
                    await _syncService.PullServerChangesAsync();
                }

                await LoadCompetitionsAsync();
                await InvokeAsync(StateHasChanged);

                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Info,
                    Summary = L["DataUpdated"],
                    Detail = L["CompetitionUpdatedByAnotherUser"]
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
        await Task.WhenAll(LoadCompetitionsAsync(), LoadAuxiliaryDataAsync());
        _isLoading = false;
    }

    private async Task LoadCompetitionsAsync()
    {
        _competitions = await CompetitionService.GetAllCompetitionsAsync();
    }

    private async Task LoadAuxiliaryDataAsync()
    {
        _disciplines = await CompetitionService.GetDisciplinesAsync();
        _categories = await CompetitionService.GetCategoriesAsync();
    }

    private void HandleAdd()
    {
        _editingCompetition = new CompetitionEditDto { Id = Guid.NewGuid(), StartDate = DateTime.Today, EndDate = DateTime.Today.AddMonths(9), Color = "#1E88E5" };
        _isEditing = true;
    }

    private async Task HandleEditAsync(Guid id)
    {
        var comp = await CompetitionService.GetCompetitionForEditAsync(id);
        if (comp != null)
        {
            _editingCompetition = comp;
            _isEditing = true;
        }
    }

    private async Task HandleSaveAsync(CompetitionEditDto model)
    {
        await CompetitionService.SaveCompetitionAsync(model);

        if (_syncService != null)
        {
            await _syncService.PushPendingChangesAsync();
        }

        if (HubConnection.State == HubConnectionState.Connected)
        {
            await HubConnection.InvokeAsync("NotifyEntityChanged", "Competition", model.Id, "Modified");
        }

        _isEditing = false;
        await LoadCompetitionsAsync();

        NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = L["Success"], Detail = L["SavedSuccessfully"] });
    }

    private void HandleCancel()
    {
        _isEditing = false;
        _editingCompetition = new();
    }

    private async Task HandleDeleteAsync(Guid id)
    {
        var confirm = await DialogService.Confirm(L["AreYouSure"], L["Delete"], new ConfirmOptions { OkButtonText = L["Yes"], CancelButtonText = L["No"] });

        if (confirm == true)
        {
            await CompetitionService.DeleteCompetitionAsync(id);

            if (_syncService != null)
            {
                await _syncService.PushPendingChangesAsync();
            }

            if (HubConnection.State == HubConnectionState.Connected)
            {
                await HubConnection.InvokeAsync("NotifyEntityChanged", "Competition", id, "Deleted");
            }

            await LoadCompetitionsAsync();
            NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = L["Success"], Detail = L["DeletedSuccessfully"] });
        }
    }

    public async ValueTask DisposeAsync()
    {
        HubConnection.Remove("EntityChanged");
        await ValueTask.CompletedTask;
    }
}
