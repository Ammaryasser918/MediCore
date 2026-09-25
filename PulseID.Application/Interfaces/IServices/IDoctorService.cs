using PulseID.Application.DTOs.Doctor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.Interfaces.IServices
{
    public interface IDoctorService
    {
        Task<IReadOnlyList<DoctorDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<DoctorDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<DoctorDto>> GetByDepartmentAsync(Guid departmentId, CancellationToken cancellationToken = default);
        Task<DoctorDto> CreateAsync(CreateDoctorDto doctorDto, CancellationToken cancellationToken = default);
        Task<DoctorDto> UpdateDepartmentAsync(Guid id, UpdateDoctorDepartmentDto dto, CancellationToken cancellationToken = default);
        Task<DoctorDto> UpdateAsync(Guid id, UpdateDoctorDto dto, CancellationToken cancellationToken);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);


        // Approval Work Flow
        Task<DoctorDto> ApproveAsync(Guid id, Guid hospitalAdminId, CancellationToken cancellationToken = default);
        Task<DoctorDto> RejectAsync(Guid id, Guid hospitalAdminId, CancellationToken cancellationToken = default);
        Task<DoctorDto> DeactivateAsync(Guid id, Guid hospitalAdminId, CancellationToken cancellationToken = default);
        Task<DoctorDto> ReactivateAsync(Guid id, Guid hospitalAdminId, CancellationToken cancellationToken = default);

    }
}
