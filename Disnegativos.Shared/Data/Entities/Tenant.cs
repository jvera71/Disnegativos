using Disnegativos.Shared.Models;

namespace Disnegativos.Shared.Data.Entities;

public class Tenant : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    // Relaciones
    public ICollection<Customer> Customers { get; set; } = [];
}
