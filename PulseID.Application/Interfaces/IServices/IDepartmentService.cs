using PulseID.Application.DTOs.Department;
using PulseID.Application.DTOs.Doctor;

namespace PulseID.Application.Interfaces.IServices;

public interface IDepartmentService
{
    Task<IReadOnlyList<DepartmentDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<DepartmentDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<DepartmentDto> GetDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<DepartmentDto> CreateDepartmentAsync(Guid hospitalId, CreateDepartmentDto dto, CancellationToken cancellationToken = default);

    Task<DepartmentDto> UpdateAsync(Guid id, UpdateDepartmentDto dto, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DoctorDto>> GetDoctorsAsync(Guid departmentId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DepartmentDto>> GetAllIncludingDeletedAsync(CancellationToken cancellationToken = default);

    Task<DepartmentDto> AssignDoctorToDepartmentAsync(Guid departmentId, Guid doctorId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DepartmentDto>> GetByHospitalAsync(Guid hospitalId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DepartmentDto>> GetByHospitalIncludingDeletedAsync(Guid hospitalId, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}