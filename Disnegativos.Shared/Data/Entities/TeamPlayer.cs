using Disnegativos.Shared.Models;

namespace Disnegativos.Shared.Data.Entities;

public class TeamPlayer : BaseEntity
{
    public Guid TeamId { get; set; }
    public Team Team { get; set; } = null!;

    public Guid PlayerId { get; set; }
    public Player Player { get; set; } = null!;

    public Guid? FieldPositionId { get; set; }
    public FieldPosition? FieldPosition { get; set; }

    public string? JerseyNumber { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? ActivationDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
