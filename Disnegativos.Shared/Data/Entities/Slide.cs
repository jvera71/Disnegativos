using Disnegativos.Shared.Models;

namespace Disnegativos.Shared.Data.Entities;

public class Slide : BaseEntity
{
    public Guid ReportId { get; set; }
    public Report Report { get; set; } = null!;

    public Guid UserId { get; set; }

    public Guid? ActionId { get; set; }
    public MatchAction? Action { get; set; }

    public Guid? TeamId { get; set; }
    public Team? Team { get; set; }

    public Guid? PlayerId { get; set; }
    public Player? Player { get; set; }

    public Guid? ButtonId { get; set; }
    public Button? Button { get; set; }

    public int SlideType { get; set; } = 0;
    public string? Notes { get; set; }
    public int SortOrder { get; set; } = 0;
    public double Duration { get; set; } = 5.0;

    public string? ChapterHtml { get; set; }
    public string? BodyHtml { get; set; }
    public string? HeaderHtml { get; set; }
    public string? NoteHtml { get; set; }

    public bool ShowChapter { get; set; } = false;
    public bool ShowBody { get; set; } = false;
    public bool ShowHeader { get; set; } = false;
    public bool ShowNote { get; set; } = false;
    public bool ShowSlideNumber { get; set; } = true;
    public bool ShowBackgroundImage { get; set; } = false;
    public string? BackgroundImagePath { get; set; }
    
    public string? JsonDrawings { get; set; }
    public string? JsonZoom { get; set; }
    
    public bool IsFavorite { get; set; } = false;
    public bool IsExpanded { get; set; } = false;
}
