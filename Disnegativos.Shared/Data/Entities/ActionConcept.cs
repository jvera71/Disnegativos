using Disnegativos.Shared.Models;

namespace Disnegativos.Shared.Data.Entities;

public class ActionConcept : BaseEntity
{
    public Guid ActionId { get; set; }
    public MatchAction Action { get; set; } = null!;

    public Guid ConceptId { get; set; }
    public Concept Concept { get; set; } = null!;

    public Guid? ButtonId { get; set; }
    public Button? Button { get; set; }

    public string? ConceptName { get; set; }
    public int SortOrder { get; set; } = 0;
}
