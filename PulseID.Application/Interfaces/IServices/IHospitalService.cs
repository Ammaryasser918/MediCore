using PulseID.Application.DTOs.Department;
using PulseID.Application.DTOs.Hospital;

namespace PulseID.Application.Interfaces.IServices;

public interface IHospitalService
{
    Task<IReadOnlyList<HospitalDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<HospitalDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<HospitalDto> CreateAsync(CreateHospitalDto hospitalDto, CancellationToken cancellationToken = default);

    Task<HospitalDto> UpdateAsync(Guid id, UpdateHospitalDto dto, CancellationToken cancellationToken = default);

    Task<HospitalDto> UpdateLicenseDocumentAsync(Guid id, string NewLicenseDocumentPath, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DepartmentDto>> GetDepartmentsAsync(Guid hospitalId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DepartmentDto>> GetDepartmentsWithDetailsAsync(Guid hospitalId, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<HospitalDto> ApproveAsync(Guid id, Guid adminId, CancellationToken cancellationToken = default);

    Task<HospitalDto> RejectAsync(Guid id, CancellationToken cancellationToken = default);

    Task<HospitalDto> DeactivateAsync(Guid id, CancellationToken cancellationToken = default);

    Task<HospitalDto> ReactivateAsync(Guid id, CancellationToken cancellationToken = default);
}