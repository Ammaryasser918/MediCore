using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PulseID.Application.DTOs.HospitalAdmin;

namespace PulseID.Application.Interfaces.IServices
{
    public interface IHospitalAdminService
    {
        Task<HospitalAdminDto> CreateHospitalAdminAsync(CreateHospitalAdminDto dto, CancellationToken cancellationToken = default);
        Task<HospitalAdminDto> GetById(Guid id, CancellationToken cancellationToken = default);
    }
}
