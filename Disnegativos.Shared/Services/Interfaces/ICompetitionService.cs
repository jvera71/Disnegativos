using Disnegativos.Shared.DTOs;
using Disnegativos.Shared.Data.Entities;

namespace Disnegativos.Shared.Services.Interfaces;

public interface ICompetitionService
{
    Task<List<CompetitionDto>> GetAllCompetitionsAsync();
    Task<CompetitionEditDto?> GetCompetitionForEditAsync(Guid id);
    Task SaveCompetitionAsync(CompetitionEditDto dto);
    Task DeleteCompetitionAsync(Guid id);
    Task<List<SportDiscipline>> GetDisciplinesAsync();
    Task<List<SportCategory>> GetCategoriesAsync();
}
