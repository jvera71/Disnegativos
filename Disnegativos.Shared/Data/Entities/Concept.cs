using Disnegativos.Shared.Models;

namespace Disnegativos.Shared.Data.Entities;

public class Concept : BaseEntity
{
    public Guid? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public Guid? PanelId { get; set; }
    public Panel? Panel { get; set; }

    public string Name { get; set; } = string.Empty;
    public int? CategoryType { get; set; }
    
    public bool IsToggle { get; set; } = false;
    public bool RequiresTeam { get; set; } = false;
    public bool RequiresPlayer { get; set; } = false;
    public bool RequiresZone { get; set; } = false;
    public bool ShowStatistics { get; set; } = false;
    public bool IsOptionMode { get; set; } = false;
    public string? OptionGroupName { get; set; }
}
