using Microsoft.AspNetCore.Http;
using MediCore.Application.DTOs.Doctor;
using MediCore.Application.DTOs.Pharmacist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Application.Interfaces.IServices
{
    public interface IDoctorService
    {
        Task<IReadOnlyList<DoctorDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<DoctorDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<DoctorDto> GetByUserIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<DoctorDto>> GetByDepartmentAsync(Guid departmentId, CancellationToken cancellationToken = default);
        Task<DoctorDto> CreateAsync(CreateDoctorDto doctorDto, CancellationToken cancellationToken = default);
        Task<DoctorDto> UpdateDepartmentAsync(Guid id, UpdateDoctorDepartmentDto dto, CancellationToken cancellationToken = default);
        Task RemoveFromDepartment(Guid doctorId, CancellationToken cancellationToken = default);
        Task<DoctorDto> UpdateAsync(Guid id, UpdateDoctorDto dto, CancellationToken cancellationToken);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task HardDeleteAsync(Guid doctorId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<DoctorDto>> GetDeletedDoctorsAsync(CancellationToken cancellationToken = default);
        Task RestoreDoctorAsync(Guid doctorId, CancellationToken cancellationToken = default);
        Task<DoctorDto> SetPersonalImageAsync(Guid id, IFormFile file, CancellationToken cancellationToken = default);
        Task<DoctorDto> RemovePersonalImageAsync(Guid id, CancellationToken cancellationToken = default);


        // Approval Work Flow
        Task<DoctorDto> ApproveAsync(Guid id, Guid hospitalAdminId, CancellationToken cancellationToken = default);
        Task<DoctorDto> RejectAsync(Guid id, Guid hospitalAdminId, CancellationToken cancellationToken = default);
        Task<DoctorDto> DeactivateAsync(Guid id, Guid hospitalAdminId, CancellationToken cancellationToken = default);
        Task<DoctorDto> ReactivateAsync(Guid id, Guid hospitalAdminId, CancellationToken cancellationToken = default);

    }
}
