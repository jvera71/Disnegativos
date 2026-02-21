namespace Disnegativos.Shared.DTOs;

public record PlayerDto(
    Guid Id,
    string FirstName,
    string LastName,
    string? Nickname,
    string? Email,
    bool IsActive
)
{
    public string FullName => $"{FirstName} {LastName}".Trim();
}

public class PlayerEditDto
{
    public Guid Id { get; set; }
    public Guid SportDisciplineId { get; set; }
    public Guid? OrganizationId { get; set; }
    public Guid? DefaultTeamId { get; set; }
    public Guid? FieldPositionId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public string? Nickname { get; set; }
    public DateTime DateOfBirth { get; set; } = DateTime.Today.AddYears(-20);
    public int Gender { get; set; }
    public Guid? CountryId { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? JerseyNumber { get; set; }
    public string? Height { get; set; }
    public string? Weight { get; set; }
    public string? PreferredFoot { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime ActivationDate { get; set; } = DateTime.Now;
    public string? Notes { get; set; }
}

public class PlayerTeamAssignmentDto
{
    public Guid Id { get; set; }
    public Guid ProjectPlayerId { get; set; }
    public Guid TeamId { get; set; }
    public string? TeamName { get; set; }
    public string? JerseyNumber { get; set; }
    public Guid? FieldPositionId { get; set; }
    public string? FieldPositionName { get; set; }
    public bool IsActive { get; set; } = true;
}

