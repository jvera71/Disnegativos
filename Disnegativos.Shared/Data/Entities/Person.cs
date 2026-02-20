using Disnegativos.Shared.Models;

namespace Disnegativos.Shared.Data.Entities;

public class Person : BaseEntity
{
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public Guid ServicePlanId { get; set; }
    public Guid SportDisciplineId { get; set; }
    public SportDiscipline SportDiscipline { get; set; } = null!;

    public Guid? RoleId { get; set; }
    public PersonRole? Role { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public int Gender { get; set; }

    public Guid? CountryId { get; set; }
    public Country? Country { get; set; }

    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? ImageFileId { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }
}
