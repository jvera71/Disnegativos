using Disnegativos.Shared.Models;

namespace Disnegativos.Shared.Data.Entities;

public class Team : BaseEntity
{
    public Guid TenantId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid ServicePlanId { get; set; }
    public Guid SportDisciplineId { get; set; }
    public Guid SportCategoryId { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Alias { get; set; }
    public string? ImageFileId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime ActivationDate { get; set; }
    public string? Notes { get; set; }
}
