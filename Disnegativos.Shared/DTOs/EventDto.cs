using System;

namespace Disnegativos.Shared.DTOs;

public class EventDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public string HomeTeamName { get; set; } = string.Empty;
    public string AwayTeamName { get; set; } = string.Empty;
    public string? Result { get; set; }
    public bool IsActive { get; set; }
}
