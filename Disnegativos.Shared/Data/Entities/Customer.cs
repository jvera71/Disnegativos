using Disnegativos.Shared.Models;

namespace Disnegativos.Shared.Data.Entities;

public class Customer : BaseEntity
{
    public Guid TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public string? Alias { get; set; }
    public string? FiscalAddress { get; set; }
    public string? ExtendedAddress { get; set; }
    
    public Guid? CountryId { get; set; }
    public Country? Country { get; set; }
    
    public string? Province { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? TaxId { get; set; } // NIF
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? LogoFileId { get; set; }
    public bool IsOnline { get; set; }
    public string? Notes { get; set; }
    
    public string Status { get; set; } = string.Empty;
    public DateTime StatusDate { get; set; }
    
    public ICollection<SubCustomer> SubCustomers { get; set; } = [];
}
