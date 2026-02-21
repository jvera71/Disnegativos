using Microsoft.EntityFrameworkCore;
using Disnegativos.Shared.Data;
using Disnegativos.Shared.Data.Entities;
using Disnegativos.Shared.DTOs;
using Disnegativos.Shared.Interfaces;
using Disnegativos.Shared.Services.Interfaces;

namespace Disnegativos.Shared.Services.Implementations;

public class CompetitionService : ICompetitionService
{
    private readonly DisnegativosDbContext _db;

    public CompetitionService(DisnegativosDbContext db)
    {
        _db = db;
    }

    public async Task<List<CompetitionDto>> GetAllCompetitionsAsync()
    {
        return await _db.Competitions
            .Where(c => !c.IsArchived)
            .OrderByDescending(c => c.StartDate)
            .ThenBy(c => c.Title)
            .Select(c => new CompetitionDto(
                c.Id,
                c.Title,
                c.SportDisciplineId,
                _db.SportDisciplines.Where(s => s.Id == c.SportDisciplineId).Select(s => s.NameKey).FirstOrDefault(),
                c.SportCategoryId,
                _db.SportCategories.Where(cat => cat.Id == c.SportCategoryId).Select(cat => cat.Name).FirstOrDefault(),
                c.StartDate,
                c.EndDate,
                c.Color,
                c.IsActive,
                c.IsPrivate,
                c.ShowInCalendar
            ))
            .ToListAsync();
    }

    public async Task<CompetitionEditDto?> GetCompetitionForEditAsync(Guid id)
    {
        var c = await _db.Competitions.FindAsync(id);
        if (c == null || c.IsArchived) return null;

        return new CompetitionEditDto
        {
            Id = c.Id,
            Title = c.Title,
            SportDisciplineId = c.SportDisciplineId,
            SportCategoryId = c.SportCategoryId,
            TeamId = c.TeamId,
            StartDate = c.StartDate,
            EndDate = c.EndDate,
            Notes = c.Notes,
            Color = c.Color,
            IsActive = c.IsActive,
            IsPrivate = c.IsPrivate,
            ShowInCalendar = c.ShowInCalendar
        };
    }

    public async Task SaveCompetitionAsync(CompetitionEditDto dto)
    {
        var competition = await _db.Competitions.FindAsync(dto.Id);
        bool isNew = competition == null;

        if (isNew)
        {
            competition = new Competition { Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id };
            _db.Competitions.Add(competition);
        }

        competition!.Title = dto.Title;
        competition.SportDisciplineId = dto.SportDisciplineId;
        competition.SportCategoryId = dto.SportCategoryId;
        competition.TeamId = dto.TeamId;
        competition.StartDate = dto.StartDate;
        competition.EndDate = dto.EndDate;
        competition.Notes = dto.Notes;
        competition.Color = dto.Color;
        competition.IsActive = dto.IsActive;
        competition.IsPrivate = dto.IsPrivate;
        competition.ShowInCalendar = dto.ShowInCalendar;

        await _db.SaveChangesAsync();
    }

    public async Task DeleteCompetitionAsync(Guid id)
    {
        var competition = await _db.Competitions.FindAsync(id);
        if (competition == null) return;

        _db.Competitions.Remove(competition);
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
}
