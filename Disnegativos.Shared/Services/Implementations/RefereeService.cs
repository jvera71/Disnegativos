using Microsoft.EntityFrameworkCore;
using Disnegativos.Shared.Data;
using Disnegativos.Shared.Data.Entities;
using Disnegativos.Shared.DTOs;
using Disnegativos.Shared.Interfaces;
using Disnegativos.Shared.Services.Interfaces;

namespace Disnegativos.Shared.Services.Implementations;

public class RefereeService : IRefereeService
{
    private readonly DisnegativosDbContext _db;

    public RefereeService(DisnegativosDbContext db)
    {
        _db = db;
    }

    public async Task<List<RefereeDto>> GetAllRefereesAsync()
    {
        return await _db.Referees
            .Where(r => !r.IsArchived)
            .OrderBy(r => r.LastName)
            .ThenBy(r => r.FirstName)
            .Select(r => new RefereeDto(
                r.Id,
                r.FirstName,
                r.LastName,
                r.LicenseNumber,
                r.Category,
                r.Email,
                r.Phone,
                r.CountryId,
                r.Country != null ? r.Country.NameKey : null,
                r.IsActive
            ))
            .ToListAsync();
    }

    public async Task<RefereeEditDto?> GetRefereeForEditAsync(Guid id)
    {
        var r = await _db.Referees.FindAsync(id);
        if (r == null || r.IsArchived) return null;

        return new RefereeEditDto
        {
            Id = r.Id,
            FirstName = r.FirstName,
            LastName = r.LastName,
            LicenseNumber = r.LicenseNumber,
            Category = r.Category,
            Email = r.Email,
            Phone = r.Phone,
            CountryId = r.CountryId,
            IsActive = r.IsActive,
            Notes = r.Notes
        };
    }

    public async Task SaveRefereeAsync(RefereeEditDto dto)
    {
        var referee = await _db.Referees.FindAsync(dto.Id);
        bool isNew = referee == null;

        if (isNew)
        {
            referee = new Referee { Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id };
            _db.Referees.Add(referee);
        }

        referee!.FirstName = dto.FirstName;
        referee.LastName = dto.LastName;
        referee.LicenseNumber = dto.LicenseNumber;
        referee.Category = dto.Category;
        referee.Email = dto.Email;
        referee.Phone = dto.Phone;
        referee.CountryId = dto.CountryId;
        referee.IsActive = dto.IsActive;
        referee.Notes = dto.Notes;

        await _db.SaveChangesAsync();
    }

    public async Task DeleteRefereeAsync(Guid id)
    {
        var referee = await _db.Referees.FindAsync(id);
        if (referee == null) return;

        _db.Referees.Remove(referee);
        await _db.SaveChangesAsync();
    }
}
