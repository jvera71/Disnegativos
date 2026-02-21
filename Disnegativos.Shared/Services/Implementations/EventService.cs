using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Disnegativos.Shared.DTOs;
using Disnegativos.Shared.Interfaces;
using Disnegativos.Shared.Services.Interfaces;
using Disnegativos.Shared.Data.Entities;
using Disnegativos.Shared.Data;

namespace Disnegativos.Shared.Services.Implementations;

public class EventService : IEventService
{
    private readonly DisnegativosDbContext _context;
    private readonly ITimeZoneService _timeZoneService;

    public EventService(DisnegativosDbContext context, ITimeZoneService timeZoneService)
    {
        _context = context;
        _timeZoneService = timeZoneService;
    }

    public async Task<List<EventDto>> GetAllEventsAsync()
    {
        var events = await _context.Events
            .Include(e => e.HomeTeam)
            .Include(e => e.AwayTeam)
            .OrderByDescending(e => e.StartDate)
            .ToListAsync();

        return events.Select(e => new EventDto
        {
            Id = e.Id,
            Name = e.Name,
            StartDate = e.StartDate,
            StartTime = e.StartTime,
            HomeTeamName = e.HomeTeam.Name,
            AwayTeamName = e.AwayTeam?.Name ?? string.Empty,
            Result = e.Result,
            IsActive = e.IsActive
        }).ToList();
    }

    public async Task<EventEditDto?> GetEventForEditAsync(Guid id)
    {
        var e = await _context.Events.FindAsync(id);
        if (e == null) return null;

        // Covertir UTC a TimeLocal del usuario que le ha dado a editar
        var combinedUtc = e.StartDate.Add(e.StartTime);
        var localDateTime = _timeZoneService.ToUserTime(combinedUtc);

        return new EventEditDto
        {
            Id = e.Id,
            Name = e.Name,
            LocalStartDate = localDateTime.Date,
            LocalStartTime = localDateTime.TimeOfDay,
            HomeTeamId = e.HomeTeamId,
            AwayTeamId = e.AwayTeamId,
            Result = e.Result,
            Notes = e.Notes,
            IsActive = e.IsActive
        };
    }

    public async Task SaveEventAsync(EventEditDto dto)
    {
        Event evt;
        if (dto.Id.HasValue && dto.Id.Value != Guid.Empty)
        {
            evt = await _context.Events.FindAsync(dto.Id.Value) 
                ?? throw new InvalidOperationException("Event not found");
        }
        else
        {
            evt = new Event();
            _context.Events.Add(evt);
        }

        // Combinar fecha local y hora local, y convertirlas a UTC absoluto para DataBase
        var localDateTime = dto.LocalStartDate!.Value.Add(dto.LocalStartTime!.Value);
        var utcDateTime = _timeZoneService.ToUtc(localDateTime);

        evt.Name = dto.Name;
        evt.StartDate = utcDateTime.Date;
        evt.StartTime = utcDateTime.TimeOfDay;
        evt.HomeTeamId = dto.HomeTeamId!.Value;
        evt.AwayTeamId = dto.AwayTeamId;
        evt.Result = dto.Result;
        evt.Notes = dto.Notes;
        evt.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteEventAsync(Guid id)
    {
        var dbEvent = await _context.Events.FindAsync(id);
        if (dbEvent != null)
        {
            _context.Events.Remove(dbEvent);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<TeamDto>> GetAvailableTeamsAsync()
    {
        var teams = await _context.Teams
            .OrderBy(t => t.Name)
            .ToListAsync();

        return teams.Select(t => new TeamDto(
            t.Id,
            t.Name,
            t.Alias,
            null,
            null,
            t.IsActive
        )).ToList();
    }
}
