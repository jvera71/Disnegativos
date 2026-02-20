using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Disnegativos.Shared.DTOs;

namespace Disnegativos.Shared.Services.Interfaces;

public interface IEventService
{
    Task<List<EventDto>> GetAllEventsAsync();
    Task<EventEditDto?> GetEventForEditAsync(Guid id);
    Task SaveEventAsync(EventEditDto dto);
    Task DeleteEventAsync(Guid id);
    
    // Auxiliary
    Task<List<TeamDto>> GetAvailableTeamsAsync();
}
