using PulseID.Application.DTOs.EmergencyContact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.Interfaces.IServices
{
    public interface IEmergencyContactService
    {
        Task<IEnumerable<EmergencyContactDto>> GetPatientContactsAsync(Guid? patientId, CancellationToken cancellationToken = default);
        Task<EmergencyContactDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<EmergencyContactDto> CreateAsync(CreateEmergencyContactDto dto, Guid? patientId, CancellationToken cancellationToken = default);
        Task<EmergencyContactDto> UpdateAsync(Guid id, UpdateEmergencyContactDto dto, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task HardDeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
