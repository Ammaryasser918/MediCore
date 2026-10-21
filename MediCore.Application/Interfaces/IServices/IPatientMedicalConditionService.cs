using MediCore.Application.DTOs.Allergies;
using MediCore.Application.DTOs.ChronicDiseases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Application.Interfaces.IServices
{
    public interface IPatientMedicalConditionService
    {
        Task<IReadOnlyList<PatientAllergyDto>> GetPatientAllergiesAsync(Guid? patientId, CancellationToken cancellationToken = default);
        Task<PatientAllergyDto> AddAllergyAsync(Guid? patientId, AddPatientAllergyDto dto, CancellationToken cancellationToken = default);
        Task RemoveAllergyAsync(Guid? patientId, Guid allergyId, CancellationToken cancellationToken = default);



        Task<IReadOnlyList<PatientChronicDiseaseDto>> GetPatientChronicDiseasesAsync(Guid? patientId, CancellationToken cancellationToken = default);
        Task<PatientChronicDiseaseDto> AddChronicDiseaseAsync(Guid? patientId, AddPateintChronicDiseaseDto dto, CancellationToken cancellationToken = default);
        Task RemoveChronicDiseasesAsync(Guid? patientId, Guid chronicDiseaseId, CancellationToken cancellationToken = default);

    }
}
