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

namespace Disnegativos.Shared.Pages.Players;

public partial class PlayersPage : ComponentBase, IAsyncDisposable
{
    [Inject] private IPlayerService PlayerService { get; set; } = default!;
    [Inject] private NotificationService NotificationService { get; set; } = default!;
    [Inject] private DialogService DialogService { get; set; } = default!;
    [Inject] private HubConnection HubConnection { get; set; } = default!;
    [Inject] private IServiceProvider ServiceProvider { get; set; } = default!;

    private List<PlayerDto> _players = [];
    private List<SportDiscipline> _disciplines = [];
    private List<CountryDto> _countries = [];
    private List<PlayerTeamAssignmentDto> _assignments = [];
    private List<TeamDto> _allTeams = [];
    private List<FieldPosition> _fieldPositions = [];

    private bool _isLoading = true;
    private bool _isEditing = false;
    private PlayerEditDto _editingPlayer = new();
    private ISyncService? _syncService;

    protected override async Task OnInitializedAsync()
    {
        _syncService = ServiceProvider.GetService<ISyncService>();

        await LoadDataAsync();

        HubConnection.On<string, Guid, string>("EntityChanged", async (entityType, entityId, changeType) =>
        {
            if (entityType == "Player" || entityType == "TeamPlayer")
            {
                if (_syncService != null)
                {
                    await _syncService.PullServerChangesAsync();
                }

                await LoadPlayersAsync();
                if (_isEditing && _editingPlayer.Id == entityId || entityType == "TeamPlayer")
                {
                    await LoadAssignmentDataAsync(_editingPlayer.Id);
                }
                await InvokeAsync(StateHasChanged);

                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Info,
                    Summary = L["DataUpdated"],
                    Detail = L["PlayerUpdatedByAnotherUser"]
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
        await Task.WhenAll(LoadPlayersAsync(), LoadAuxiliaryDataAsync());
        _isLoading = false;
    }

    private async Task LoadPlayersAsync()
    {
        _players = await PlayerService.GetAllPlayersAsync();
    }

    private async Task LoadAuxiliaryDataAsync()
    {
        _disciplines = await PlayerService.GetDisciplinesAsync();
        _countries = await PlayerService.GetCountriesAsync();
    }

    private async Task LoadAssignmentDataAsync(Guid playerId)
    {
        if (playerId == Guid.Empty) return;
        _assignments = await PlayerService.GetPlayerTeamAssignmentsAsync(playerId);
        _allTeams = await PlayerService.GetTeamsAsync(_editingPlayer.SportDisciplineId);
        _fieldPositions = await PlayerService.GetFieldPositionsAsync(_editingPlayer.SportDisciplineId);
    }

    private void HandleAdd()
    {
        _editingPlayer = new PlayerEditDto { Id = Guid.NewGuid(), ActivationDate = DateTime.Now };
        _assignments = [];
        _isEditing = true;
    }

    private async Task HandleEditAsync(Guid id)
    {
        var player = await PlayerService.GetPlayerForEditAsync(id);
        if (player != null)
        {
            _editingPlayer = player;
            await LoadAssignmentDataAsync(id);
            _isEditing = true;
        }
    }

    private async Task HandleSaveAsync(PlayerEditDto model)
    {
        await PlayerService.SavePlayerAsync(model);

        if (_syncService != null)
        {
            await _syncService.PushPendingChangesAsync();
        }

        if (HubConnection.State == HubConnectionState.Connected)
        {
            await HubConnection.InvokeAsync("NotifyEntityChanged", "Player", model.Id, "Modified");
        }

        _isEditing = false;
        await LoadPlayersAsync();

        NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = L["Success"], Detail = L["SavedSuccessfully"] });
    }

    private async Task HandleSaveAssignmentAsync(PlayerTeamAssignmentDto assignment)
    {
        await PlayerService.SavePlayerTeamAssignmentAsync(assignment);

        if (_syncService != null)
        {
            await _syncService.PushPendingChangesAsync();
        }

        if (HubConnection.State == HubConnectionState.Connected)
        {
            await HubConnection.InvokeAsync("NotifyEntityChanged", "TeamPlayer", assignment.Id, "Modified");
        }

        await LoadAssignmentDataAsync(_editingPlayer.Id);
        NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = L["Success"], Detail = L["SavedSuccessfully"] });
    }

    private async Task HandleRemoveAssignmentAsync(Guid assignmentId)
    {
        await PlayerService.RemovePlayerTeamAssignmentAsync(assignmentId);

        if (_syncService != null)
        {
            await _syncService.PushPendingChangesAsync();
        }

        if (HubConnection.State == HubConnectionState.Connected)
        {
            await HubConnection.InvokeAsync("NotifyEntityChanged", "TeamPlayer", assignmentId, "Deleted");
        }

        await LoadAssignmentDataAsync(_editingPlayer.Id);
        NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = L["Success"], Detail = L["DeletedSuccessfully"] });
    }

    private void HandleCancel()
    {
        _isEditing = false;
        _editingPlayer = new();
    }

    private async Task HandleDeleteAsync(Guid id)
    {
        var confirm = await DialogService.Confirm(L["AreYouSure"], L["Delete"], new ConfirmOptions { OkButtonText = L["Yes"], CancelButtonText = L["No"] });

        if (confirm == true)
        {
            await PlayerService.DeletePlayerAsync(id);

            if (_syncService != null)
            {
                await _syncService.PushPendingChangesAsync();
            }

            if (HubConnection.State == HubConnectionState.Connected)
            {
                await HubConnection.InvokeAsync("NotifyEntityChanged", "Player", id, "Deleted");
            }

            await LoadPlayersAsync();
            NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = L["Success"], Detail = L["DeletedSuccessfully"] });
        }
    }

    public async ValueTask DisposeAsync()
    {
        HubConnection.Remove("EntityChanged");
        await ValueTask.CompletedTask;
    }
}
