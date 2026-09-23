using PulseID.Application.DTOs.Labs;
using PulseID.Domain.Enums;

namespace PulseID.Application.Interfaces.Services;

public interface ILabTestRequestService
{
    Task<IReadOnlyList<LabTestRequestDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<LabTestRequestDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LabTestRequestDto>> GetByPatientAsync(Guid patientId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LabTestRequestDto>> GetByDoctorAsync(Guid doctorId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LabTestRequestDto>> GetByLabAsync(Guid labId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LabTestRequestDto>> GetByStatusAsync(LabTestRequestStatus status, CancellationToken cancellationToken = default);

    Task<LabTestRequestDto> CreateAsync(CreateLabTestRequestDto dto, CancellationToken cancellationToken = default);

    Task<LabTestRequestDto> UpdateAsync(Guid id, UpdateLabTestRequestDto dto, CancellationToken cancellationToken = default);

    Task AssignLabAsync(Guid id, Guid labId, CancellationToken cancellationToken = default);

    Task SendToLabAsync(Guid id, CancellationToken cancellationToken = default);

    Task CollectSampleAsync(Guid id, CancellationToken cancellationToken = default);

    Task StartProcessingAsync(Guid id, CancellationToken cancellationToken = default);

    Task UploadResultAsync(Guid id, string resultFileUrl, CancellationToken cancellationToken = default);
    Task UploadPatientResultAsync(Guid id, string resultFileUrl, CancellationToken cancellationToken = default);
    Task RejectAsync(Guid id, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}