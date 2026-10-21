using MediCore.Application.Common.Interfaces;
using MediCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Application.Interfaces.Repositories
{
    public interface IMedicalRecordRepository : IRepository<MedicalRecord>
    {
        Task<IReadOnlyList<MedicalRecord>> GetByPatientAsync(Guid patientId, CancellationToken cancellationToken = default);
        Task<MedicalRecord?> GetWithDetailsAsync(Guid medicalRecordId, CancellationToken cancellationToken = default);

        Task<MedicalRecord?> GetLatestVersionAsync(Guid rootRecordId, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<MedicalRecord>> GetByDoctorAsync(Guid doctorId, CancellationToken cancellationToken = default);

    }
}
