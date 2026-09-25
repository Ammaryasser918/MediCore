using PulseID.Application.DTOs.Pharmacist;
using PulseID.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.Interfaces.IServices
{
    public interface IPharmacistService
    {
        Task<IReadOnlyList<PharmacistDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<PharmacistDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PharmacistDto> CreateAsync(CreatePharmacistDto dto, CancellationToken cancellationToken = default);
        Task<PharmacistDto> AssignToPharmacyAsync(Guid id, AssignPharmacistToPharmacyDto dto, CancellationToken cancellationToken = default);
        Task<PharmacistDto> UpdatePharmacistDetailsAsync(Guid id, UpdatePharmacistDto dto, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);


        Task<PharmacistDto> ApproveAsync(Guid id, Guid pharmacyAdminId, CancellationToken cancellationToken = default);
        Task<PharmacistDto> RejectAsync(Guid id, Guid pharmacyAdminId, CancellationToken cancellationToken = default);
        Task<PharmacistDto> DeactivateAsync(Guid id, Guid pharmacyAdminId, CancellationToken cancellationToken = default);
        Task<PharmacistDto> ReactivateAsync(Guid id, Guid pharmacyAdminId, CancellationToken cancellationToken = default);


    }
}
