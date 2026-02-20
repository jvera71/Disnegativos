using Disnegativos.Shared.Models;

namespace Disnegativos.Shared.Data.Entities;

public class User : BaseEntity
{
    public Guid TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
    
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    
    public Guid? SubCustomerId { get; set; }
    public SubCustomer? SubCustomer { get; set; }

    public Guid ServicePlanId { get; set; }
    public ServicePlan ServicePlan { get; set; } = null!;

    public string Email { get; set; } = string.Empty;
    public string? PasswordHash { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    
    public Guid CountryId { get; set; }
    public Country Country { get; set; } = null!;

    public DateTime? DateOfBirth { get; set; }
    public int? Gender { get; set; }
    public string? Phone { get; set; }
    public string? NotificationEmail { get; set; }
    public string? UserType { get; set; }
    public string PreferredLocale { get; set; } = "es";
    public string TimeZoneId { get; set; } = "Europe/Madrid";
    public string? AvatarFileId { get; set; }
    
    public bool IsActive { get; set; } = true;
    public DateTime? ActivationDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    
    public bool IsBlocked { get; set; }
    public DateTime? BlockedDate { get; set; }
    public string? Notes { get; set; }
}
