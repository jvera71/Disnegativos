namespace Disnegativos.Shared.DTOs;

public record RefereeDto(
    Guid Id,
    string FirstName,
    string LastName,
    string? LicenseNumber,
    string? Category,
    string? Email,
    string? Phone,
    Guid? CountryId,
    string? CountryName,
    bool IsActive
)
{
    public string FullName => $"{FirstName} {LastName}".Trim();
}

public class RefereeEditDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? LicenseNumber { get; set; }
    public string? Category { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public Guid? CountryId { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }
}
