using Disnegativos.Shared.Models;

namespace Disnegativos.Shared.Data.Entities;

public class Report : BaseEntity
{
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public Guid ServicePlanId { get; set; }
    public Guid UserId { get; set; }

    public Guid? EventId { get; set; }
    public Event? Event { get; set; }

    public Guid? AnalysisId { get; set; }
    public Analysis? Analysis { get; set; }

    public Guid? SportDisciplineId { get; set; }
    public SportDiscipline? SportDiscipline { get; set; }

    public string Name { get; set; } = string.Empty;
    public DateTime ReportDate { get; set; }
    public double? TotalDurationSec { get; set; }
    public string? MediaUrl { get; set; }
    public string? Notes { get; set; }

    public bool IsPrivate { get; set; } = false;
    public string? Password { get; set; }
    
    public bool ShowHeaders { get; set; } = true;
    public bool ShowNotes { get; set; } = true;
    public bool ShowSlideNumbers { get; set; } = true;
    public bool ShowChapters { get; set; } = true;
    public bool ShowBody { get; set; } = true;
    public bool ShowCover { get; set; } = true;
    public bool ShowCredits { get; set; } = true;
    public bool ShowPlayerOnAction { get; set; } = false;
    public bool ShowActionTime { get; set; } = false;
    public bool ShowBadge { get; set; } = false;
    public int SendStatus { get; set; } = 0;

    // Relaciones
    public ICollection<Slide> Slides { get; set; } = [];
}
