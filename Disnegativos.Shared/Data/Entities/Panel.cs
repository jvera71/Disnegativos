using Disnegativos.Shared.Models;

namespace Disnegativos.Shared.Data.Entities;

public class Panel : BaseEntity
{
    public Guid TemplateId { get; set; }
    public Template Template { get; set; } = null!;

    public Guid? UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    public string? BackgroundColor { get; set; }
    public string? ForegroundColor { get; set; }
    public int? FontSize { get; set; }
    
    public int SortOrder { get; set; } = 0;
    public bool IsVisible { get; set; } = true;
    public bool IsActive { get; set; } = true;
    
    public int SecondsBeforeClip { get; set; } = 0;
    public int SecondsAfterClip { get; set; } = 0;
    
    public int? LevelCount { get; set; }
    public int? Padding { get; set; }

    // Relaciones
    public ICollection<Block> Blocks { get; set; } = [];
}
