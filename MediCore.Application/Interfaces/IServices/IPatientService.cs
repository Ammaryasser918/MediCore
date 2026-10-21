using Microsoft.AspNetCore.Http;
using MediCore.Application.DTOs.Doctor;
using MediCore.Application.DTOs.Patient;
using MediCore.Application.DTOs.Pharmacist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Application.Interfaces.IServices
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
        Task HardDeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<PatientDto>> GetDeletedPatientsAsync(CancellationToken cancellationToken = default);
        Task RestorePatientAsync(Guid patientId, CancellationToken cancellationToken = default);
        Task<PatientDto> SetPersonalImageAsync(Guid id, IFormFile file, CancellationToken cancellationToken = default);
        Task<PatientDto> RemovePersonalImageAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
