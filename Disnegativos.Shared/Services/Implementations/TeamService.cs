using Microsoft.EntityFrameworkCore;
using Disnegativos.Shared.Data;
using Disnegativos.Shared.Data.Entities;
using Disnegativos.Shared.DTOs;
using Disnegativos.Shared.Interfaces;
using Disnegativos.Shared.Services.Interfaces;

namespace Disnegativos.Shared.Services.Implementations;

public class TeamService : ITeamService
{
    private readonly DisnegativosDbContext _db;

    public TeamService(DisnegativosDbContext db)
    {
        _db = db;
    }

    public async Task<List<TeamDto>> GetAllTeamsAsync()
    {
        return await _db.Teams
            .Where(t => !t.IsArchived)
            .OrderBy(t => t.Name)
            .Select(t => new TeamDto(
                t.Id,
                t.Name,
                t.Alias,
                _db.SportCategories.Where(c => c.Id == t.SportCategoryId).Select(c => c.Name).FirstOrDefault(),
                _db.SportDisciplines.Where(s => s.Id == t.SportDisciplineId).Select(s => s.NameKey).FirstOrDefault(),
                t.IsActive
            ))
            .ToListAsync();
    }

    public async Task<TeamEditDto?> GetTeamForEditAsync(Guid id)
    {
        var t = await _db.Teams.FindAsync(id);
        if (t == null || t.IsArchived) return null;

        return new TeamEditDto
        {
            Id = t.Id,
            Name = t.Name,
            Alias = t.Alias,
            SportDisciplineId = t.SportDisciplineId,
            SportCategoryId = t.SportCategoryId,
            IsActive = t.IsActive,
            ActivationDate = t.ActivationDate,
            Notes = t.Notes
        };
    }

    public async Task SaveTeamAsync(TeamEditDto dto)
    {
        var team = await _db.Teams.FindAsync(dto.Id);
        bool isNew = team == null;

        if (isNew)
        {
            team = new Team { Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id };
            _db.Teams.Add(team);
        }

        team!.Name = dto.Name;
        team.Alias = dto.Alias;
        team.SportDisciplineId = dto.SportDisciplineId;
        team.SportCategoryId = dto.SportCategoryId;
        team.IsActive = dto.IsActive;
        team.ActivationDate = dto.ActivationDate;
        team.Notes = dto.Notes;

        await _db.SaveChangesAsync();
    }

    public async Task DeleteTeamAsync(Guid id)
    {
        var team = await _db.Teams.FindAsync(id);
        if (team == null) return;

        _db.Teams.Remove(team);
        await _db.SaveChangesAsync();
    }

    public async Task<List<TeamPlayerDto>> GetTeamPlayersAsync(Guid teamId)
    {
        return await _db.TeamPlayers
            .Where(tp => tp.TeamId == teamId && !tp.IsArchived)
            .Select(tp => new TeamPlayerDto
            {
                Id = tp.Id,
                TeamId = tp.TeamId,
                PlayerId = tp.PlayerId,
                PlayerFullName = tp.Player.FirstName + " " + tp.Player.LastName,
                JerseyNumber = tp.JerseyNumber,
                FieldPositionId = tp.FieldPositionId,
                FieldPositionName = tp.FieldPosition != null ? tp.FieldPosition.Name : null,
                IsActive = tp.IsActive
            })
            .ToListAsync();
    }

    public async Task SaveTeamPlayerAsync(TeamPlayerDto dto)
    {
        var tp = await _db.TeamPlayers.FindAsync(dto.Id);
        bool isNew = tp == null;

        if (isNew)
        {
            tp = new TeamPlayer { Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id };
            _db.TeamPlayers.Add(tp);
        }

        tp!.TeamId = dto.TeamId;
        tp.PlayerId = dto.PlayerId;
        tp.JerseyNumber = dto.JerseyNumber;
        tp.FieldPositionId = dto.FieldPositionId;
        tp.IsActive = dto.IsActive;

        await _db.SaveChangesAsync();
    }

    public async Task RemoveTeamPlayerAsync(Guid teamPlayerId)
    {
        var tp = await _db.TeamPlayers.FindAsync(teamPlayerId);
        if (tp == null) return;

        _db.TeamPlayers.Remove(tp);
        await _db.SaveChangesAsync();
    }

    public async Task<List<SportDiscipline>> GetDisciplinesAsync()
    {
        return await _db.SportDisciplines.Where(s => !s.IsArchived).OrderBy(s => s.NameKey).ToListAsync();
    }

    public async Task<List<SportCategory>> GetCategoriesAsync()
    {
        return await _db.SportCategories.Where(c => !c.IsArchived).OrderBy(c => c.Name).ToListAsync();
    }

    public async Task<List<PlayerDto>> GetAvailablePlayersAsync()
    {
        return await _db.Players
            .Where(p => !p.IsArchived && p.IsActive)
            .OrderBy(p => p.FirstName)
            .Select(p => new PlayerDto(p.Id, p.FirstName, p.LastName, p.Nickname, p.Email, p.IsActive))
            .ToListAsync();
    }

    public async Task<List<FieldPosition>> GetFieldPositionsAsync(Guid disciplineId)
    {
        return await _db.FieldPositions
            .Where(fp => fp.SportDisciplineId == disciplineId && !fp.IsArchived)
            .OrderBy(fp => fp.Name)
            .ToListAsync();
    }
}
