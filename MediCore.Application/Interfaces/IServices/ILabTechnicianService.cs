using Microsoft.AspNetCore.Http;
using MediCore.Application.DTOs.Doctor;
using MediCore.Application.DTOs.Labs;
using MediCore.Application.DTOs.Pharmacist;

namespace MediCore.Application.Interfaces.Services;

public interface ILabTechnicianService
{
    Task<IReadOnlyList<LabTechnicianDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<LabTechnicianDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<LabTechnicianDto?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LabTechnicianDto>> GetByLabAsync(Guid labId, CancellationToken cancellationToken = default);

    Task<LabTechnicianDto> CreateAsync(CreateLabTechnicianDto dto, CancellationToken cancellationToken = default);

    Task UpdateAsync(Guid id, UpdateLabTechnicianDto dto, CancellationToken cancellationToken = default);

    Task TransferToLabAsync(Guid id, Guid labId, CancellationToken cancellationToken = default);

    Task ApproveAsync(Guid id, Guid labAdminId, CancellationToken cancellationToken = default);

    Task RejectAsync(Guid id, Guid labAdminId, CancellationToken cancellationToken = default);

    Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default);

    Task ReactivateAsync(Guid id, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task HardDeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LabTechnicianDto>> GetDeletedTechniciansAsync(CancellationToken cancellationToken = default);

    Task RestoreTechnicianAsync(Guid id, CancellationToken cancellationToken = default);

    Task<LabTechnicianDto> SetPersonalImageAsync(Guid id, IFormFile file, CancellationToken cancellationToken = default);
    Task<LabTechnicianDto> RemovePersonalImageAsync(Guid id, CancellationToken cancellationToken = default);
}