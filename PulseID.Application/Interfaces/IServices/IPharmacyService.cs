using PulseID.Application.DTOs.Pharmacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.Interfaces.IServices
{
    public interface IPharmacyService
    {
        Task<IReadOnlyList<PharmacyDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<PharmacyDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<PharmacyDto> CreateAsync(CreatePharmacyDto dto, CancellationToken cancellationToken = default);
        

        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

        Task<PharmacyDto> ApproveAsync(Guid id, Guid adminId, CancellationToken cancellationToken = default);
        Task<PharmacyDto> RejectAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PharmacyDto> DeactivateAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PharmacyDto> ReactivateAsync(Guid id, CancellationToken cancellationToken = default);

    }
}
