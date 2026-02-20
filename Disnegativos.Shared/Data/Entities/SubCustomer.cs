using Disnegativos.Shared.Models;

namespace Disnegativos.Shared.Data.Entities;

public class SubCustomer : BaseEntity
{
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? ExtendedAddress { get; set; }
    
    public Guid? CountryId { get; set; }
    public Country? Country { get; set; }
    
    public string? Province { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? TaxId { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? LogoFileId { get; set; }
    public bool IsOnline { get; set; }
    public string? Notes { get; set; }
    
    public string Status { get; set; } = string.Empty;
    public DateTime StatusDate { get; set; }
}
