using Disnegativos.Shared.Models;

namespace Disnegativos.Shared.Data.Entities;

public class FieldPosition : BaseEntity
{
    public Guid SportDisciplineId { get; set; }
    public SportDiscipline SportDiscipline { get; set; } = null!;

    public Guid? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public string Name { get; set; } = string.Empty;
}
