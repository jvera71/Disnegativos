using Disnegativos.Shared.DTOs;

namespace Disnegativos.Shared.Services.Interfaces;

public interface ICountryService
{
    Task<List<CountryDto>> GetAllCountriesAsync();
    Task<CountryEditDto?> GetCountryForEditAsync(Guid id);
    Task SaveCountryAsync(CountryEditDto dto);
    Task DeleteCountryAsync(Guid id);
}
