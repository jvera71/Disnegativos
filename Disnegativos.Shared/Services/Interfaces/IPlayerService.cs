using Disnegativos.Shared.Data.Entities;
using Disnegativos.Shared.DTOs;

namespace Disnegativos.Shared.Services.Interfaces;

public interface IPlayerService
{
    // Player CRUD
    Task<List<PlayerDto>> GetAllPlayersAsync();
    Task<PlayerEditDto?> GetPlayerForEditAsync(Guid id);
    Task SavePlayerAsync(PlayerEditDto dto);
    Task DeletePlayerAsync(Guid id);

    // Multiple Team Assignments
    Task<List<PlayerTeamAssignmentDto>> GetPlayerTeamAssignmentsAsync(Guid playerId);
    Task SavePlayerTeamAssignmentAsync(PlayerTeamAssignmentDto dto);
    Task RemovePlayerTeamAssignmentAsync(Guid assignmentId);

    // Helpers
    Task<List<SportDiscipline>> GetDisciplinesAsync();
    Task<List<OrganizationDto>> GetOrganizationsAsync();
    Task<List<TeamDto>> GetTeamsAsync(Guid disciplineId);
    Task<List<FieldPosition>> GetFieldPositionsAsync(Guid disciplineId);
    Task<List<CountryDto>> GetCountriesAsync();
}
