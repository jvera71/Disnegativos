namespace Disnegativos.Shared.DTOs;

public record OrganizationDto(
    Guid Id,
    string Name,
    string? City,
    string? Email,
    string? Phone,
    string? Website,
    Guid SportDisciplineId,
    string? SportDisciplineName,
    bool IsActive
);

public class OrganizationEditDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? ExtendedAddress { get; set; }
    public string? PostalCode { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public Guid? CountryId { get; set; }
    public Guid SportDisciplineId { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Website { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? ActivationDate { get; set; }
    public string? Notes { get; set; }
}
