using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Radzen;
using Disnegativos.Shared.DTOs;
using Disnegativos.Shared.Services.Interfaces;

using Microsoft.AspNetCore.SignalR.Client;

namespace Disnegativos.Shared.Pages.Events;

public partial class EventsPage : ComponentBase, IAsyncDisposable
{
    // Solo el Container puede inyectar los servicios
    [Inject] 
    private IEventService EventService { get; set; } = default!;

    [Inject]
    private HubConnection HubConnection { get; set; } = default!;

    private List<EventDto> _events = [];
    private List<TeamDto> _availableTeams = [];
    private bool _isLoading = true;
    
    // UI State
    private bool _isEditing = false;
    private EventEditDto _editingEvent = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadEventsAsync();
        _availableTeams = await EventService.GetAvailableTeamsAsync();

        // ── Suscribirse a cambios en tiempo real ──
        HubConnection.On<string, Guid, string>("EntityChanged", async (entityType, entityId, changeType) =>
        {
            // Si el cambio es en un Evento (y no somos nosotros mismos recargando)
            if (entityType == "Event")
            {
                await LoadEventsAsync();
                await InvokeAsync(StateHasChanged);
            }
        });

        // Asegurar que la conexión esté iniciada
        if (HubConnection.State == HubConnectionState.Disconnected)
        {
            try { await HubConnection.StartAsync(); }
            catch { /* Ignorar si falla (ej: offline en MAUI) */ }
        }
    }

    private async Task LoadEventsAsync()
    {
        _isLoading = true;
        _events = await EventService.GetAllEventsAsync();
        _isLoading = false;
    }

    // Funciones Handler (reciben los eventos desde los Presentationals)
    
    private void HandleAdd()
    {
        _editingEvent = new EventEditDto
        {
            LocalStartDate = DateTime.Now.Date,
            LocalStartTime = DateTime.Now.TimeOfDay
        };
        _isEditing = true;
    }

    private async Task HandleEditAsync(Guid eventId)
    {
        var e = await EventService.GetEventForEditAsync(eventId);
        if (e != null)
        {
            _editingEvent = e;
            _isEditing = true;
        }
    }

    private async Task HandleSaveAsync(EventEditDto model)
    {
        await EventService.SaveEventAsync(model);
        
        // Notificar a otros usuarios
        if (HubConnection.State == HubConnectionState.Connected)
        {
            await HubConnection.InvokeAsync("NotifyEntityChanged", "Event", model.Id, "Modified");
        }

        _isEditing = false;
        await LoadEventsAsync();
    }

    private void HandleCancel()
    {
        _isEditing = false;
        _editingEvent = new();
    }

    private async Task HandleDeleteAsync(Guid eventId)
    {
        // Delete Soft
        await EventService.DeleteEventAsync(eventId);

        // Notificar a otros usuarios
        if (HubConnection.State == HubConnectionState.Connected)
        {
            await HubConnection.InvokeAsync("NotifyEntityChanged", "Event", eventId, "Deleted");
        }

        await LoadEventsAsync();
    }

    public async ValueTask DisposeAsync()
    {
        // En una app real podríamos querer quitar el handler específico con .Off(...)
        // pero dado que el HubConnection suele ser Singleton/Scoped, es mejor dejarlo o limpiar.
        HubConnection.Remove("EntityChanged");
        await ValueTask.CompletedTask;
    }
}
