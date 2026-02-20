using Disnegativos.Shared.Models;

namespace Disnegativos.Shared.Data.Entities;

public class Block : BaseEntity
{
    public Guid PanelId { get; set; }
    public Panel Panel { get; set; } = null!;

    public Guid? UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public string? BackgroundColor { get; set; }
    public string? ForegroundColor { get; set; }
    public int? FontSize { get; set; }
    
    public int SortOrder { get; set; } = 0;
    public bool IsVisible { get; set; } = true;
    public bool IsHeaderVisible { get; set; } = true;
    public bool ShowCounter { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public bool IsFixed { get; set; } = false;
    public bool IsOptionMode { get; set; } = false;
    
    public int SecondsBeforeClip { get; set; } = 0;
    public int SecondsAfterClip { get; set; } = 0;
    public bool ShowStatistics { get; set; } = false;

    // Relaciones
    public ICollection<Button> Buttons { get; set; } = [];
}
