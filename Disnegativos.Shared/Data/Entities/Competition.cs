using Disnegativos.Shared.Models;

namespace Disnegativos.Shared.Data.Entities;

public class Competition : BaseEntity
{
    public Guid CustomerId { get; set; }
    public Guid ServicePlanId { get; set; }
    public Guid SportDisciplineId { get; set; }
    public Guid SportCategoryId { get; set; }
    public Guid? UserId { get; set; }
    public Guid? TeamId { get; set; }
    
    public string Title { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Notes { get; set; }
    public string? Color { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsPrivate { get; set; } = false;
    public bool ShowInCalendar { get; set; } = true;
    public string? ImageFileId { get; set; }
    
    public ICollection<Event> Events { get; set; } = [];
}
