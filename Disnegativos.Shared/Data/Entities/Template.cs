using Disnegativos.Shared.Models;

namespace Disnegativos.Shared.Data.Entities;

public class Template : BaseEntity
{
    public Guid CustomerId { get; set; }
    public Guid ServicePlanId { get; set; }
    
    public Guid? SportDisciplineId { get; set; }
    public SportDiscipline? SportDiscipline { get; set; }

    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Author { get; set; }

    public bool IsDefault { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public int Status { get; set; } // Ajustar según Enum si existe
    public bool ToggleableBlocks { get; set; } = false;
    public bool CollapsibleBlocks { get; set; } = false;

    // Relaciones
    public ICollection<Panel> Panels { get; set; } = [];
}
