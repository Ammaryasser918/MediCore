using PulseID.Application.DTOs.Labs;

namespace PulseID.Application.Interfaces.Services;

public interface ILabService
{
    Task<IReadOnlyList<LabDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<LabDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<LabDto?> GetWithRequestsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<LabDto> CreateAsync(CreateLabDto dto, CancellationToken cancellationToken = default);

    Task<LabDto> UpdateAsync(Guid id, UpdateLabDto dto, CancellationToken cancellationToken = default);

    Task UpdateLicenseDocumentAsync(Guid id, string documentPath, CancellationToken cancellationToken = default);

    Task ApproveAsync(Guid id, Guid hospitalAdminId, CancellationToken cancellationToken = default);

    Task RejectAsync(Guid id, Guid labAdminId, CancellationToken cancellationToken = default);

    Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default);

    Task ReactivateAsync(Guid id, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task HardDeleteAsync(Guid labId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LabDto>> GetDeletedLabsAsync(CancellationToken cancellationToken = default);
    Task RestoreLabAsync(Guid id, CancellationToken cancellationToken = default);
}