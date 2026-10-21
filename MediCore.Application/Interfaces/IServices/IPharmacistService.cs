using Microsoft.AspNetCore.Http;
using MediCore.Application.DTOs.Doctor;
using MediCore.Application.DTOs.Patient;
using MediCore.Application.DTOs.Pharmacist;
using MediCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Application.Interfaces.IServices
{
    public interface IPharmacistService
    {
        Task<IReadOnlyList<PharmacistDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<PharmacistDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PharmacistDto> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<PharmacistDto> CreateAsync(CreatePharmacistDto dto, CancellationToken cancellationToken = default);
        Task<PharmacistDto> AssignToPharmacyAsync(Guid id, AssignPharmacistToPharmacyDto dto, CancellationToken cancellationToken = default);
        Task<PharmacistDto> UpdatePharmacistDetailsAsync(Guid id, UpdatePharmacistDto dto, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task HardDeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task StartShiftAsync(Guid? pharmacistId, CancellationToken cancellationToken = default);
        Task EndShiftAsync(Guid? pharmacistId, CancellationToken cancellationToken = default);
        Task<PharmacistShiftInfoDto> GetShiftInfoAsync(Guid? pharmacistId, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<PharmacistDto>> GetDeletedPharmacistsAsync(CancellationToken cancellationToken = default);
        Task RestorePharmacistAsync(Guid pharmacistId, CancellationToken cancellationToken = default);

        Task<PharmacistDto> ApproveAsync(Guid id, Guid pharmacyAdminId, CancellationToken cancellationToken = default);
        Task<PharmacistDto> RejectAsync(Guid id, Guid pharmacyAdminId, CancellationToken cancellationToken = default);
        Task<PharmacistDto> DeactivateAsync(Guid id, Guid pharmacyAdminId, CancellationToken cancellationToken = default);
        Task<PharmacistDto> ReactivateAsync(Guid id, Guid pharmacyAdminId, CancellationToken cancellationToken = default);

        Task<PharmacistDto> SetPersonalImageAsync(Guid id, IFormFile file, CancellationToken cancellationToken = default);
        Task<PharmacistDto> RemovePersonalImageAsync(Guid id, CancellationToken cancellationToken = default);


    }
}
