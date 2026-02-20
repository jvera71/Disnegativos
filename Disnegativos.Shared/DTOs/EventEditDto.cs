using System;
using System.ComponentModel.DataAnnotations;

namespace Disnegativos.Shared.DTOs;

public class EventEditDto
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "NameRequired")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "DateRequired")]
    public DateTime? LocalStartDate { get; set; }

    [Required(ErrorMessage = "TimeRequired")]
    public TimeSpan? LocalStartTime { get; set; }

    // Necesitamos que seleccione al menos el equipo local
    [Required(ErrorMessage = "HomeTeamRequired")]
    public Guid? HomeTeamId { get; set; }

    public Guid? AwayTeamId { get; set; }

    public string? Result { get; set; }

    public string? Notes { get; set; }
    
    public bool IsActive { get; set; } = true;
}
