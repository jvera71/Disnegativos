using Disnegativos.Shared.Data.Entities;
using Disnegativos.Shared.DTOs;

namespace Disnegativos.Shared.Services.Interfaces;

public interface IOrganizationService
{
    Task<List<OrganizationDto>> GetAllOrganizationsAsync();
    Task<OrganizationEditDto?> GetOrganizationForEditAsync(Guid id);
    Task SaveOrganizationAsync(OrganizationEditDto dto);
    Task DeleteOrganizationAsync(Guid id);
    Task<List<SportDiscipline>> GetDisciplinesAsync();
    Task<List<CountryDto>> GetCountriesAsync();
}
