using Disnegativos.Shared.DTOs;

namespace Disnegativos.Shared.Services.Interfaces;

public interface IRefereeService
{
    Task<List<RefereeDto>> GetAllRefereesAsync();
    Task<RefereeEditDto?> GetRefereeForEditAsync(Guid id);
    Task SaveRefereeAsync(RefereeEditDto dto);
    Task DeleteRefereeAsync(Guid id);
}
