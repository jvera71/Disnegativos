namespace Disnegativos.Shared.DTOs;

public record CompetitionDto(
    Guid Id,
    string Title,
    Guid SportDisciplineId,
    string? SportDisciplineName,
    Guid SportCategoryId,
    string? SportCategoryName,
    DateTime? StartDate,
    DateTime? EndDate,
    string? Color,
    bool IsActive,
    bool IsPrivate,
    bool ShowInCalendar
);

public class CompetitionEditDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public Guid SportDisciplineId { get; set; }
    public Guid SportCategoryId { get; set; }
    public Guid? TeamId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Notes { get; set; }
    public string? Color { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsPrivate { get; set; } = false;
    public bool ShowInCalendar { get; set; } = true;
}
