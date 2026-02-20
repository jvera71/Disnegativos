using Disnegativos.Shared.Models;

namespace Disnegativos.Shared.Data.Entities;

public class PersonRole : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    
    public Guid? SportDisciplineId { get; set; }
    public SportDiscipline? SportDiscipline { get; set; }

    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }
}
