using Microsoft.EntityFrameworkCore;
using Disnegativos.Shared.Data;
using Disnegativos.Shared.Data.Entities;
using Disnegativos.Shared.DTOs;
using Disnegativos.Shared.Interfaces;
using Disnegativos.Shared.Services.Interfaces;

namespace Disnegativos.Shared.Services.Implementations;

public class OrganizationService : IOrganizationService
{
    private readonly DisnegativosDbContext _db;

    public OrganizationService(DisnegativosDbContext db)
    {
        _db = db;
    }

    public async Task<List<OrganizationDto>> GetAllOrganizationsAsync()
    {
        return await _db.Organizations
            .Include(o => o.SportDiscipline)
            .Where(o => !o.IsArchived)
            .OrderBy(o => o.Name)
            .Select(o => new OrganizationDto(
                o.Id,
                o.Name,
                o.City,
                o.Email,
                o.Phone,
                o.Website,
                o.SportDisciplineId,
                o.SportDiscipline.NameKey,
                o.IsActive
            ))
            .ToListAsync();
    }

    public async Task<OrganizationEditDto?> GetOrganizationForEditAsync(Guid id)
    {
        var o = await _db.Organizations.FindAsync(id);
        if (o == null || o.IsArchived) return null;

        return new OrganizationEditDto
        {
            Id = o.Id,
            Name = o.Name,
            Address = o.Address,
            ExtendedAddress = o.ExtendedAddress,
            PostalCode = o.PostalCode,
            City = o.City,
            Province = o.Province,
            CountryId = o.CountryId,
            SportDisciplineId = o.SportDisciplineId,
            Email = o.Email,
            Phone = o.Phone,
            Website = o.Website,
            IsActive = o.IsActive,
            ActivationDate = o.ActivationDate,
            Notes = o.Notes
        };
    }

    public async Task SaveOrganizationAsync(OrganizationEditDto dto)
    {
        var org = await _db.Organizations.FindAsync(dto.Id);
        bool isNew = org == null;

        if (isNew)
        {
            org = new Organization { Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id };
            _db.Organizations.Add(org);
        }

        org!.Name = dto.Name;
        org.Address = dto.Address;
        org.ExtendedAddress = dto.ExtendedAddress;
        org.PostalCode = dto.PostalCode;
        org.City = dto.City;
        org.Province = dto.Province;
        org.CountryId = dto.CountryId;
        org.SportDisciplineId = dto.SportDisciplineId;
        org.Email = dto.Email;
        org.Phone = dto.Phone;
        org.Website = dto.Website;
        org.IsActive = dto.IsActive;
        org.ActivationDate = dto.ActivationDate;
        org.Notes = dto.Notes;

        await _db.SaveChangesAsync();
    }

    public async Task DeleteOrganizationAsync(Guid id)
    {
        var org = await _db.Organizations.FindAsync(id);
        if (org == null) return;

        _db.Organizations.Remove(org);
        await _db.SaveChangesAsync();
    }

    public async Task<List<SportDiscipline>> GetDisciplinesAsync()
    {
        return await _db.SportDisciplines.Where(s => !s.IsArchived).OrderBy(s => s.NameKey).ToListAsync();
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
