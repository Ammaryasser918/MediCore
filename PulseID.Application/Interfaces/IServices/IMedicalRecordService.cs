using PulseID.Application.DTOs.Doctor;
using PulseID.Application.DTOs.MedicalRecord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.Interfaces.IServices
{
    public interface IMedicalRecordService
    {
        Task<MedicalRecordDto> CreateAsync(CreateMedicalRecordDto dto, CancellationToken cancellationToken = default);

        Task<MedicalRecordDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<MedicalRecordListItemDto>> GetByPatientAsync(Guid patientId, bool latestVersionsOnly = true, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<MedicalRecordListItemDto>> GetByDoctorAsync(Guid? doctorId, bool latestVersionsOnly = true, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<MedicalRecordVersionDto>> GetVersionHistoryAsync(Guid anyVersionId, CancellationToken cancellationToken = default);

        Task<MedicalRecordDto> CreateNewVersionAsync(Guid currentVersionId, CreateMedicalRecordVersionDto dto, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<MedicalRecordDto>> GetDeletedMedicalRecordsAsync(CancellationToken cancellationToken = default);
        Task RestoreMedicalRecordAsync(Guid medicalRecordId, CancellationToken cancellationToken = default);

    }
}
