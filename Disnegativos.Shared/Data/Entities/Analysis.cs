using Disnegativos.Shared.Models;

namespace Disnegativos.Shared.Data.Entities;

public class Analysis : BaseEntity
{
    public Guid EventId { get; set; }
    public Event Event { get; set; } = null!;

    public Guid TemplateId { get; set; }
    public Template Template { get; set; } = null!;

    public Guid UserId { get; set; }
    
    public Guid? AnalyzedTeamId { get; set; }
    public Team? AnalyzedTeam { get; set; }

    public Guid? OpponentTeamId { get; set; }
    public Team? OpponentTeam { get; set; }

    public string Name { get; set; } = string.Empty;
    public TimeSpan TotalDuration { get; set; }
    
    public bool HasVideo { get; set; } = false;
    public bool IsMultiVideo { get; set; } = false;
    public bool IsLive { get; set; } = false;
    public bool IsPrivate { get; set; } = false;
    public bool IsFinished { get; set; } = false;
    public DateTime? FinishedDate { get; set; }
    
    public bool AnalyzeOpponents { get; set; } = false;
    public bool AnalyzeBoth { get; set; } = false;
    
    public int? Status { get; set; }
    public string? Notes { get; set; }
    public string? JsonConfig { get; set; }
    public int? CurrentPeriod { get; set; }

    // Relaciones
    public ICollection<AnalysisMedia> MediaFiles { get; set; } = [];
    public ICollection<GamePeriod> GamePeriods { get; set; } = [];
    public ICollection<MatchAction> Actions { get; set; } = [];
}
