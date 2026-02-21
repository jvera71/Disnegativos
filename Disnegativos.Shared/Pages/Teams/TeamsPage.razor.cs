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

namespace Disnegativos.Shared.Pages.Teams;

public partial class TeamsPage : ComponentBase, IAsyncDisposable
{
    [Inject] private ITeamService TeamService { get; set; } = default!;
    [Inject] private NotificationService NotificationService { get; set; } = default!;
    [Inject] private DialogService DialogService { get; set; } = default!;
    [Inject] private HubConnection HubConnection { get; set; } = default!;
    [Inject] private IServiceProvider ServiceProvider { get; set; } = default!;

    private List<TeamDto> _teams = [];
    private List<SportDiscipline> _disciplines = [];
    private List<SportCategory> _categories = [];
    private List<TeamPlayerDto> _teamPlayers = [];
    private List<PlayerDto> _availablePlayers = [];
    private List<FieldPosition> _fieldPositions = [];

    private bool _isLoading = true;
    private bool _isEditing = false;
    private TeamEditDto _editingTeam = new();
    private ISyncService? _syncService;

    protected override async Task OnInitializedAsync()
    {
        _syncService = ServiceProvider.GetService<ISyncService>();

        await LoadDataAsync();

        HubConnection.On<string, Guid, string>("EntityChanged", async (entityType, entityId, changeType) =>
        {
            if (entityType == "Team" || entityType == "TeamPlayer")
            {
                if (_syncService != null)
                {
                    await _syncService.PullServerChangesAsync();
                }

                await LoadTeamsAsync();
                if (_isEditing && _editingTeam.Id == entityId || entityType == "TeamPlayer")
                {
                    await LoadRosterDataAsync(_editingTeam.Id);
                }
                await InvokeAsync(StateHasChanged);

                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Info,
                    Summary = L["DataUpdated"],
                    Detail = L["TeamUpdatedByAnotherUser"]
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
        await Task.WhenAll(LoadTeamsAsync(), LoadAuxiliaryDataAsync());
        _isLoading = false;
    }

    private async Task LoadTeamsAsync()
    {
        _teams = await TeamService.GetAllTeamsAsync();
    }

    private async Task LoadAuxiliaryDataAsync()
    {
        _disciplines = await TeamService.GetDisciplinesAsync();
        _categories = await TeamService.GetCategoriesAsync();
        _availablePlayers = await TeamService.GetAvailablePlayersAsync();
    }

    private async Task LoadRosterDataAsync(Guid teamId)
    {
        if (teamId == Guid.Empty) return;
        _teamPlayers = await TeamService.GetTeamPlayersAsync(teamId);
        _fieldPositions = await TeamService.GetFieldPositionsAsync(_editingTeam.SportDisciplineId);
    }

    private void HandleAdd()
    {
        _editingTeam = new TeamEditDto { Id = Guid.NewGuid(), ActivationDate = DateTime.Now };
        _teamPlayers = [];
        _isEditing = true;
    }

    private async Task HandleEditAsync(Guid id)
    {
        var team = await TeamService.GetTeamForEditAsync(id);
        if (team != null)
        {
            _editingTeam = team;
            await LoadRosterDataAsync(id);
            _isEditing = true;
        }
    }

    private async Task HandleSaveAsync(TeamEditDto model)
    {
        await TeamService.SaveTeamAsync(model);

        if (_syncService != null)
        {
            await _syncService.PushPendingChangesAsync();
        }

        if (HubConnection.State == HubConnectionState.Connected)
        {
            await HubConnection.InvokeAsync("NotifyEntityChanged", "Team", model.Id, "Modified");
        }

        _isEditing = false;
        await LoadTeamsAsync();

        NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = L["Success"], Detail = L["SavedSuccessfully"] });
    }

    private async Task HandleSavePlayerAsync(TeamPlayerDto tp)
    {
        await TeamService.SaveTeamPlayerAsync(tp);

        if (_syncService != null)
        {
            await _syncService.PushPendingChangesAsync();
        }

        if (HubConnection.State == HubConnectionState.Connected)
        {
            await HubConnection.InvokeAsync("NotifyEntityChanged", "TeamPlayer", tp.Id, "Modified");
        }

        await LoadRosterDataAsync(_editingTeam.Id);
        NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = L["Success"], Detail = L["SavedSuccessfully"] });
    }

    private async Task HandleRemovePlayerAsync(Guid tpId)
    {
        await TeamService.RemoveTeamPlayerAsync(tpId);

        if (_syncService != null)
        {
            await _syncService.PushPendingChangesAsync();
        }

        if (HubConnection.State == HubConnectionState.Connected)
        {
            await HubConnection.InvokeAsync("NotifyEntityChanged", "TeamPlayer", tpId, "Deleted");
        }

        await LoadRosterDataAsync(_editingTeam.Id);
        NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = L["Success"], Detail = L["DeletedSuccessfully"] });
    }

    private void HandleCancel()
    {
        _isEditing = false;
        _editingTeam = new();
    }

    private async Task HandleDeleteAsync(Guid id)
    {
        var confirm = await DialogService.Confirm(L["AreYouSure"], L["Delete"], new ConfirmOptions { OkButtonText = L["Yes"], CancelButtonText = L["No"] });

        if (confirm == true)
        {
            await TeamService.DeleteTeamAsync(id);

            if (_syncService != null)
            {
                await _syncService.PushPendingChangesAsync();
            }

            if (HubConnection.State == HubConnectionState.Connected)
            {
                await HubConnection.InvokeAsync("NotifyEntityChanged", "Team", id, "Deleted");
            }

            await LoadTeamsAsync();
            NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = L["Success"], Detail = L["DeletedSuccessfully"] });
        }
    }

    public async ValueTask DisposeAsync()
    {
        HubConnection.Remove("EntityChanged");
        await ValueTask.CompletedTask;
    }
}
