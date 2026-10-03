using PulseID.Application.DTOs.PatientMedications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.Interfaces.IServices
{
    public interface IPatientMedicationService
    {
        Task<IReadOnlyList<PatientMedicationDto>> GetPatientMedicationsAsync(Guid? patientId, bool activeOnly, CancellationToken cancellationToken = default);
        Task<PatientMedicationDto> AddMedicationAsync(Guid? patientId, AddPatientMedicationDto dto, CancellationToken cancellationToken = default);
        Task StopMedicationAsync(Guid? patientId, Guid patientMedicationId, StopPatientMedicationDto dto, CancellationToken cancellationToken = default);
    }
}
