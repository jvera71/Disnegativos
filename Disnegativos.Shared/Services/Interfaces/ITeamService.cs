using Disnegativos.Shared.Data.Entities;
using Disnegativos.Shared.DTOs;

namespace Disnegativos.Shared.Services.Interfaces;

public interface ITeamService
{
    // Team CRUD
    Task<List<TeamDto>> GetAllTeamsAsync();
    Task<TeamEditDto?> GetTeamForEditAsync(Guid id);
    Task SaveTeamAsync(TeamEditDto dto);
    Task DeleteTeamAsync(Guid id);

    // Roster Management (TeamPlayers)
    Task<List<TeamPlayerDto>> GetTeamPlayersAsync(Guid teamId);
    Task SaveTeamPlayerAsync(TeamPlayerDto dto);
    Task RemoveTeamPlayerAsync(Guid teamPlayerId);

    // Auxiliary
    Task<List<SportDiscipline>> GetDisciplinesAsync();
    Task<List<SportCategory>> GetCategoriesAsync();
    Task<List<PlayerDto>> GetAvailablePlayersAsync(); // To add to roster
    Task<List<FieldPosition>> GetFieldPositionsAsync(Guid disciplineId);
}
