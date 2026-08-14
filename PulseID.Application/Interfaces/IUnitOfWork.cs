using PulseID.Application.Interfaces.Repositories;
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
        IPharmacyRepository Pharmacys { get; }
        IMedicalRecordRepository MedicalRecords { get; }
        IPrescriptionRepository Prescriptions { get; }
        IOTPRepository Otps { get; }
        IAccessAuthorizationRepository AccessAuthorizations { get; }
        IAuditLogRepository AuditLogs { get; }
        INotificationRepository Notifications { get; }
        IAllergyRepository Allergies { get; }
        IChronicDiseaseRepository ChronicDiseases { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    }
}
