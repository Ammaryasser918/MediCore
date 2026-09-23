using PulseID.Application.Interfaces.Repositories;
using PulseID.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IPatientRepository Patients { get; }
        IDoctorRepository Doctors { get; }
        IPharmacistRepository Pharmacists { get; }
        IHospitalRepository Hospitals { get; }
        IDepartmentRepository Departments { get; }
        IPharmacyRepository Pharmacies { get; }
        IMedicalRecordRepository MedicalRecords { get; }
        IPrescriptionRepository Prescriptions { get; }
        IOTPRepository Otps { get; }
        IAccessAuthorizationRepository AccessAuthorizations { get; }
        IAuditLogRepository AuditLogs { get; }
        INotificationRepository Notifications { get; }
        IAllergyRepository Allergies { get; }
        IChronicDiseaseRepository ChronicDiseases { get; }
        ILabRepository Labs { get; }
        ILabTestRepository LabTests { get; }
        ILabTestRequestRepository LabTestRequests { get; }
        ILabTechnicianRepository LabTechnicians { get; }
        IMedicationRepository Medications { get; }

        Task TrackAsAddedAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : BaseEntity;

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    }
}
