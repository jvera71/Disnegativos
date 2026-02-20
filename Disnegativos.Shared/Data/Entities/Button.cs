using Disnegativos.Shared.Models;

namespace Disnegativos.Shared.Data.Entities;

public class Button : BaseEntity
{
    public Guid BlockId { get; set; }
    public Block Block { get; set; } = null!;

    public Guid? ConceptId { get; set; }
    public Concept? Concept { get; set; }

    public Guid? ParentButtonId { get; set; }
    public Button? ParentButton { get; set; }

    public string Name { get; set; } = string.Empty;
    public int ButtonCategoryType { get; set; } = 0;
    
    public string? BackgroundColor { get; set; }
    public string? ForegroundColor { get; set; }
    
    public double SecondsBeforeClip { get; set; } = 2.0;
    public double SecondsAfterClip { get; set; } = 2.0;
    
    public int SortOrder { get; set; } = 0;
    public bool IsVisible { get; set; } = true;
    public bool IsActive { get; set; } = true;
    public bool IsFavorite { get; set; } = false;
    public bool IsAttribute { get; set; } = false;
    public bool IsInOut { get; set; } = false;
    public bool IsToggle { get; set; } = false;
    public bool HasChildren { get; set; } = false;
    public bool RequiresTeam { get; set; } = false;
    public bool RequiresPlayer { get; set; } = false;
    public bool RequiresZone { get; set; } = false;
    public bool ShowStatistics { get; set; } = false;
    
    public string? Shortcut { get; set; }
    public string? Description { get; set; }
    public string? BackgroundImage { get; set; }

    // Relaciones
    public ICollection<Button> ChildButtons { get; set; } = [];
}
