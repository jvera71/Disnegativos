using Disnegativos.Shared.Models;

namespace Disnegativos.Shared.Data.Entities;

public class ActionPlayer : BaseEntity
{
    public Guid ActionId { get; set; }
    public MatchAction Action { get; set; } = null!;

    public Guid PlayerId { get; set; }
    public Player Player { get; set; } = null!;

    public int SortOrder { get; set; } = 0;
}
