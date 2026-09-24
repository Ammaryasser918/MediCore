using PulseID.Application.DTOs.HospitalAdmin;
using PulseID.Application.DTOs.PharmacyAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.Interfaces.IServices
{
    public interface IPharmacyAdminService
    {
        Task<PharmacyAdminDto> CreatePharmacyAdminAsync(CreatePharmacyAdminDto dto, CancellationToken cancellationToken = default);
        Task<PharmacyAdminDto> GetByIdAsync(Guid Id, CancellationToken cancellationToken = default);
    }
}
