using Disnegativos.Shared.Models;

namespace Disnegativos.Shared.Data.Entities;

public class MatchAction : BaseEntity
{
    public Guid AnalysisId { get; set; }
    public Analysis Analysis { get; set; } = null!;

    public Guid EventId { get; set; }
    public Event Event { get; set; } = null!;

    public Guid ButtonId { get; set; }
    public Button Button { get; set; } = null!;

    public Guid? UserId { get; set; }
    public Guid? TeamId { get; set; }
    public Team? Team { get; set; }

    public Guid? GamePeriodId { get; set; }
    public GamePeriod? GamePeriod { get; set; }

    public string? ButtonName { get; set; }
    public string? ButtonColor { get; set; }
    public string? Notes { get; set; }
    public string? MediaUrl { get; set; }

    public TimeSpan Timestamp { get; set; }
    public TimeSpan? TimestampEnd { get; set; }
    public double? TimestampMs { get; set; }
    public double? TimestampEndMs { get; set; }

    public double SecondsBeforeClip { get; set; } = 0;
    public double SecondsAfterClip { get; set; } = 0;
    
    public int VideoPositionMs { get; set; } = 0;
    public int VideoStartMs { get; set; } = 0;
    public int VideoEndMs { get; set; } = 0;

    public bool IsFavorite { get; set; } = false;
    public int? IsInOut { get; set; }
    
    public string? JsonAttributes { get; set; }
    public string? GeoLocation { get; set; }
    public string? GeoLocationEnd { get; set; }
    public string? Distance { get; set; }
    
    public int? SortOrder { get; set; }
    public int? SortOrderCreation { get; set; }
    public string? Score { get; set; }

    public Guid? ParentActionId { get; set; }
    public MatchAction? ParentAction { get; set; }

    public Guid? RelatedActionId { get; set; }
    public MatchAction? RelatedAction { get; set; }

    // Relaciones
    public ICollection<ActionPlayer> ActionPlayers { get; set; } = [];
    public ICollection<ActionConcept> ActionConcepts { get; set; } = [];
}
