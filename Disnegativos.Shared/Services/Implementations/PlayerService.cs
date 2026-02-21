using Microsoft.EntityFrameworkCore;
using Disnegativos.Shared.Data;
using Disnegativos.Shared.Data.Entities;
using Disnegativos.Shared.DTOs;
using Disnegativos.Shared.Interfaces;
using Disnegativos.Shared.Services.Interfaces;

namespace Disnegativos.Shared.Services.Implementations;

public class PlayerService : IPlayerService
{
    private readonly DisnegativosDbContext _db;

    public PlayerService(DisnegativosDbContext db)
    {
        _db = db;
    }

    public async Task<List<PlayerDto>> GetAllPlayersAsync()
    {
        return await _db.Players
            .Where(p => !p.IsArchived)
            .OrderBy(p => p.FirstName)
            .ThenBy(p => p.LastName)
            .Select(p => new PlayerDto(
                p.Id,
                p.FirstName,
                p.LastName ?? string.Empty,
                p.Nickname,
                p.Email,
                p.IsActive
            ))
            .ToListAsync();
    }

    public async Task<PlayerEditDto?> GetPlayerForEditAsync(Guid id)
    {
        var p = await _db.Players.FindAsync(id);
        if (p == null || p.IsArchived) return null;

        return new PlayerEditDto
        {
            Id = p.Id,
            SportDisciplineId = p.SportDisciplineId,
            OrganizationId = p.OrganizationId,
            DefaultTeamId = p.DefaultTeamId,
            FieldPositionId = p.FieldPositionId,
            FirstName = p.FirstName,
            LastName = p.LastName,
            Nickname = p.Nickname,
            DateOfBirth = p.DateOfBirth,
            Gender = p.Gender,
            CountryId = p.CountryId,
            Email = p.Email,
            Phone = p.Phone,
            JerseyNumber = p.JerseyNumber,
            Height = p.Height,
            Weight = p.Weight,
            PreferredFoot = p.PreferredFoot,
            IsActive = p.IsActive,
            ActivationDate = p.ActivationDate,
            Notes = p.Notes
        };
    }

    public async Task SavePlayerAsync(PlayerEditDto dto)
    {
        var player = await _db.Players.FindAsync(dto.Id);
        bool isNew = player == null;

        if (isNew)
        {
            player = new Player { Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id };
            _db.Players.Add(player);
        }

        player!.FirstName = dto.FirstName;
        player.LastName = dto.LastName;
        player.Nickname = dto.Nickname;
        player.SportDisciplineId = dto.SportDisciplineId;
        player.OrganizationId = dto.OrganizationId;
        player.DefaultTeamId = dto.DefaultTeamId;
        player.FieldPositionId = dto.FieldPositionId;
        player.DateOfBirth = dto.DateOfBirth;
        player.Gender = dto.Gender;
        player.CountryId = dto.CountryId;
        player.Email = dto.Email;
        player.Phone = dto.Phone;
        player.JerseyNumber = dto.JerseyNumber;
        player.Height = dto.Height;
        player.Weight = dto.Weight;
        player.PreferredFoot = dto.PreferredFoot;
        player.IsActive = dto.IsActive;
        player.ActivationDate = dto.ActivationDate;
        player.Notes = dto.Notes;

        await _db.SaveChangesAsync();
    }

    public async Task DeletePlayerAsync(Guid id)
    {
        var player = await _db.Players.FindAsync(id);
        if (player == null) return;

        _db.Players.Remove(player);
        await _db.SaveChangesAsync();
    }

    public async Task<List<PlayerTeamAssignmentDto>> GetPlayerTeamAssignmentsAsync(Guid playerId)
    {
        return await _db.TeamPlayers
            .Where(tp => tp.PlayerId == playerId && !tp.IsArchived)
            .Include(tp => tp.Team)
            .Include(tp => tp.FieldPosition)
            .Select(tp => new PlayerTeamAssignmentDto
            {
                Id = tp.Id,
                ProjectPlayerId = tp.PlayerId,
                TeamId = tp.TeamId,
                TeamName = tp.Team.Name,
                JerseyNumber = tp.JerseyNumber,
                FieldPositionId = tp.FieldPositionId,
                FieldPositionName = tp.FieldPosition != null ? tp.FieldPosition.Name : null,
                IsActive = tp.IsActive
            })
            .ToListAsync();
    }

    public async Task SavePlayerTeamAssignmentAsync(PlayerTeamAssignmentDto dto)
    {
        var tp = await _db.TeamPlayers.FindAsync(dto.Id);
        bool isNew = tp == null;

        if (isNew)
        {
            tp = new TeamPlayer { Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id };
            _db.TeamPlayers.Add(tp);
        }

        tp!.PlayerId = dto.ProjectPlayerId;
        tp.TeamId = dto.TeamId;
        tp.JerseyNumber = dto.JerseyNumber;
        tp.FieldPositionId = dto.FieldPositionId;
        tp.IsActive = dto.IsActive;

        await _db.SaveChangesAsync();
    }

    public async Task RemovePlayerTeamAssignmentAsync(Guid assignmentId)
    {
        var tp = await _db.TeamPlayers.FindAsync(assignmentId);
        if (tp == null) return;

        _db.TeamPlayers.Remove(tp);
        await _db.SaveChangesAsync();
    }

    public async Task<List<SportDiscipline>> GetDisciplinesAsync()
    {
        return await _db.SportDisciplines.Where(s => !s.IsArchived).OrderBy(s => s.NameKey).ToListAsync();
    }

    public async Task<List<OrganizationDto>> GetOrganizationsAsync()
    {
        return await _db.Organizations
            .Where(o => !o.IsArchived)
            .OrderBy(o => o.Name)
            .Select(o => new OrganizationDto(o.Id, o.Name, o.City, o.Email, o.Phone, o.Website, o.SportDisciplineId, null, o.IsActive))
            .ToListAsync();
    }

    public async Task<List<TeamDto>> GetTeamsAsync(Guid disciplineId)
    {
        return await _db.Teams
            .Where(t => t.SportDisciplineId == disciplineId && !t.IsArchived)
            .OrderBy(t => t.Name)
            .Select(t => new TeamDto(t.Id, t.Name, t.Alias, null, null, t.IsActive))
            .ToListAsync();
    }

    public async Task<List<FieldPosition>> GetFieldPositionsAsync(Guid disciplineId)
    {
        return await _db.FieldPositions
            .Where(fp => fp.SportDisciplineId == disciplineId && !fp.IsArchived)
            .OrderBy(fp => fp.Name)
            .ToListAsync();
    }

    public async Task<List<CountryDto>> GetCountriesAsync()
    {
        return await _db.Countries
            .Where(c => !c.IsArchived)
            .OrderBy(c => c.SortOrder)
            .Select(c => new CountryDto(c.Id, c.IsoCode, c.NameKey, c.NationalityKey, c.LanguageCode, c.SortOrder))
            .ToListAsync();
    }
}
