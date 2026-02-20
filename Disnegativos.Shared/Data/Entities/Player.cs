using Disnegativos.Shared.Models;

namespace Disnegativos.Shared.Data.Entities;

public class Player : BaseEntity
{
    public Guid TenantId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid ServicePlanId { get; set; }
    public Guid SportDisciplineId { get; set; }
    public SportDiscipline SportDiscipline { get; set; } = null!;

    public Guid UserId { get; set; }

    public Guid? OrganizationId { get; set; }
    public Organization? Organization { get; set; }

    public Guid? DefaultTeamId { get; set; }
    public Team? DefaultTeam { get; set; }

    public Guid? FieldPositionId { get; set; }
    public FieldPosition? FieldPosition { get; set; }

    public int PlayerType { get; set; } // Ajustar según Enum si existe
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public string? Nickname { get; set; }
    public DateTime DateOfBirth { get; set; }
    public int Gender { get; set; }

    public Guid? CountryId { get; set; }
    public Country? Country { get; set; }

    public Guid? SecondCountryId { get; set; }
    public Country? SecondCountry { get; set; }

    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? JerseyNumber { get; set; }
    public string? Height { get; set; }
    public string? Weight { get; set; }
    public string? PreferredFoot { get; set; }
    public string? ImageFileId { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime ActivationDate { get; set; }
    public string? Notes { get; set; }

    // Relaciones
    public ICollection<TeamPlayer> TeamPlayers { get; set; } = [];
}
