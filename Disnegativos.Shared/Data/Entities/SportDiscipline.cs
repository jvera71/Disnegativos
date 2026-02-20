using Disnegativos.Shared.Models;

namespace Disnegativos.Shared.Data.Entities;

public class SportDiscipline : BaseEntity
{
    public string NameKey { get; set; } = string.Empty;
    public string? ImageFileId { get; set; }
    public int SquadSize { get; set; }
    public int FieldPlayerCount { get; set; }
    public int PeriodCount { get; set; }
    public int? OvertimeCount { get; set; }
    public TimeSpan PeriodDuration { get; set; }
    public TimeSpan OvertimeDuration { get; set; }
    public bool HasGoalkeeper { get; set; } = true;
    public bool AutoOvertime { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public DateTime ActivationDate { get; set; }
    public string? Notes { get; set; }
    public string? FieldColor { get; set; }

    // Relaciones
    public ICollection<SportCategory> Categories { get; set; } = [];
    public ICollection<FieldPosition> FieldPositions { get; set; } = [];
}
