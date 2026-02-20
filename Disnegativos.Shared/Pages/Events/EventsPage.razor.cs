using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Radzen;
using Disnegativos.Shared.DTOs;
using Disnegativos.Shared.Services.Interfaces;

namespace Disnegativos.Shared.Pages.Events;

public partial class EventsPage : ComponentBase
{
    // Solo el Container puede inyectar los servicios
    [Inject] 
    private IEventService EventService { get; set; } = default!;

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
        await LoadEventsAsync();
    }
}
