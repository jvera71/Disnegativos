using Disnegativos.Shared.Models;

namespace Disnegativos.Shared.Data.Entities;

public class Referee : BaseEntity
{
    public Guid TenantId { get; set; }
    public Guid CustomerId { get; set; }
    
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? LicenseNumber { get; set; }
    public string? Category { get; set; }
    
    public string? Email { get; set; }
    public string? Phone { get; set; }
    
    public Guid? CountryId { get; set; }
    public Country? Country { get; set; }
    
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }
    
    public string FullName => $"{FirstName} {LastName}".Trim();
}
