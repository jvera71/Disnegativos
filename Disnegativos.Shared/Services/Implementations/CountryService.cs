using Microsoft.EntityFrameworkCore;
using Disnegativos.Shared.Data;
using Disnegativos.Shared.Data.Entities;
using Disnegativos.Shared.DTOs;
using Disnegativos.Shared.Interfaces;
using Disnegativos.Shared.Services.Interfaces;

namespace Disnegativos.Shared.Services.Implementations;

public class CountryService : ICountryService
{
    private readonly DisnegativosDbContext _db;

    public CountryService(DisnegativosDbContext db)
    {
        _db = db;
    }

    public async Task<List<CountryDto>> GetAllCountriesAsync()
    {
        return await _db.Countries
            .Where(c => !c.IsArchived)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.NameKey)
            .Select(c => new CountryDto(
                c.Id,
                c.IsoCode,
                c.NameKey,
                c.NationalityKey,
                c.LanguageCode,
                c.SortOrder
            ))
            .ToListAsync();
    }

    public async Task<CountryEditDto?> GetCountryForEditAsync(Guid id)
    {
        var c = await _db.Countries.FindAsync(id);
        if (c == null || c.IsArchived) return null;

        return new CountryEditDto
        {
            Id = c.Id,
            IsoCode = c.IsoCode,
            NameKey = c.NameKey,
            NationalityKey = c.NationalityKey,
            LanguageCode = c.LanguageCode,
            SortOrder = c.SortOrder
        };
    }

    public async Task SaveCountryAsync(CountryEditDto dto)
    {
        var country = await _db.Countries.FindAsync(dto.Id);
        bool isNew = country == null;

        if (isNew)
        {
            country = new Country { Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id };
            _db.Countries.Add(country);
        }

        country!.IsoCode = dto.IsoCode;
        country.NameKey = dto.NameKey;
        country.NationalityKey = dto.NationalityKey;
        country.LanguageCode = dto.LanguageCode;
        country.SortOrder = dto.SortOrder;

        await _db.SaveChangesAsync();
    }

    public async Task DeleteCountryAsync(Guid id)
    {
        var country = await _db.Countries.FindAsync(id);
        if (country == null) return;

        _db.Countries.Remove(country);
        await _db.SaveChangesAsync();
    }
}
