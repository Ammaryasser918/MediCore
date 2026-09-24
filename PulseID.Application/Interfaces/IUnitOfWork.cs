using PulseID.Application.Interfaces.Repositories;
using PulseID.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

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
        IHospitalAdminRepository HospitalAdmin { get; }
        IPharmacyAdminRepository PharmacyAdmin { get; }

        Task TrackAsAddedAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : BaseEntity;
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    }
}
