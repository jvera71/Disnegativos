using Disnegativos.Shared.Models;

namespace Disnegativos.Shared.Data.Entities;

public class Organization : BaseEntity
{
    public Guid TenantId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid ServicePlanId { get; set; }
    public Guid UserId { get; set; }
    public Guid SportDisciplineId { get; set; }
    public SportDiscipline SportDiscipline { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? ExtendedAddress { get; set; }
    public string? PostalCode { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    
    public Guid? CountryId { get; set; }
    public Country? Country { get; set; }

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Website { get; set; }
    public string? LogoFileId { get; set; }

    public bool IsOrgChart { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public DateTime? ActivationDate { get; set; }
    public string? Notes { get; set; }

    // Relaciones
    public ICollection<Team> Teams { get; set; } = [];
    public ICollection<Player> Players { get; set; } = [];
}
