using Disnegativos.Shared.Models;

namespace Disnegativos.Shared.Data.Entities;

public class GamePeriod : BaseEntity
{
    public Guid AnalysisId { get; set; }
    public Analysis Analysis { get; set; } = null!;

    public int PeriodIndex { get; set; }
    public string PeriodName { get; set; } = string.Empty;
    
    public bool IsMatchStart { get; set; } = false;
    public bool IsPeriodStart { get; set; } = false;
    public bool IsMatchEnd { get; set; } = false;
    public bool IsActive { get; set; } = true;
}
