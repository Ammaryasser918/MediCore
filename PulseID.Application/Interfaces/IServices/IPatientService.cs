using PulseID.Application.DTOs.Doctor;
using PulseID.Application.DTOs.Patient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.Interfaces.IServices
{
    public interface IPatientService
    {
        Task<IReadOnlyList<PatientDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<PatientDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PatientDto> GetByUserIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PatientDto?> GetByNationalIdAsync(string nationalId, CancellationToken cancellationToken = default);
        Task<PatientDto> CreateAsync(CreatePatientDto patientDto, CancellationToken cancellationToken = default);
        Task<PatientDto> UpdateAsync(Guid id, UpdatePatientDto patientDto, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<PatientDto>> GetDeletedPatientsAsync(CancellationToken cancellationToken = default);
        Task RestorePatientAsync(Guid patientId, CancellationToken cancellationToken = default);
    }
}
