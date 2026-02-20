using Disnegativos.Shared.Models;

namespace Disnegativos.Shared.Data.Entities;

public class Event : BaseEntity
{
    public Guid TenantId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid ServicePlanId { get; set; }
    public Guid SportDisciplineId { get; set; }
    public Guid SportCategoryId { get; set; }
    
    public Guid CompetitionId { get; set; }
    public Competition Competition { get; set; } = null!;
    
    public Guid UserId { get; set; }
    
    public Guid HomeTeamId { get; set; }
    public Team HomeTeam { get; set; } = null!;
    
    public Guid? AwayTeamId { get; set; }
    public Team? AwayTeam { get; set; }
    
    public string Name { get; set; } = string.Empty;
    public string? ShortName { get; set; }
    public bool IsHomeGame { get; set; }
    public string? Result { get; set; }
    
    public DateTime StartDate { get; set; } // UTC
    public TimeSpan StartTime { get; set; } // O DateTime almacenando UTC
    public TimeSpan? MeetingTime { get; set; }
    
    public string? Matchday { get; set; }
    public string? Phase { get; set; }
    public string? Notes { get; set; }
    public string? JsonData { get; set; }
    
    public bool IsActive { get; set; } = true;
    public DateTime ActivationDate { get; set; }
}
