namespace Disnegativos.Shared.DTOs;

public record TeamDto(
    Guid Id,
    string Name,
    string? Alias,
    string? CategoryName,
    string? DisciplineName,
    bool IsActive
);

public class TeamEditDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Alias { get; set; }
    public Guid SportDisciplineId { get; set; }
    public Guid SportCategoryId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime ActivationDate { get; set; } = DateTime.Now;
    public string? Notes { get; set; }
}

public class TeamPlayerDto
{
    public Guid Id { get; set; }
    public Guid TeamId { get; set; }
    public Guid PlayerId { get; set; }
    public string? PlayerFullName { get; set; }
    public string? JerseyNumber { get; set; }
    public Guid? FieldPositionId { get; set; }
    public string? FieldPositionName { get; set; }
    public bool IsActive { get; set; } = true;
}
