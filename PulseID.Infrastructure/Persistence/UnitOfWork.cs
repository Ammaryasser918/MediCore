using Microsoft.EntityFrameworkCore.Storage;
using PulseID.Application.Interfaces;
using PulseID.Application.Interfaces.Repositories;
using PulseID.Infrastructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        readonly AppDbContext _context;

        IPatientRepository? _patients;
        IDoctorRepository? _doctors;
        IPharmacistRepository? _pharmacists;
        IHospitalRepository? _hospitals;
        IDepartmentRepository? _departments;
        IPharmacyRepository? _pharmacies;
        IMedicalRecordRepository? _medicalRecords;
        IPrescriptionRepository? _prescriptions;
        IOTPRepository? _otps;
        IAccessAuthorizationRepository? _accessAuthorizations;
        IAuditLogRepository? _auditLogs;
        INotificationRepository? _notifications;
        IAllergyRepository? _allergies;
        IChronicDiseaseRepository? _chronicDiseases;
        IHospitalAdminRepository? _hospitalAdmin;
        IPharmacyAdminRepository? _pharmacyAdmin;
        IEmergencyContactRepository? _emergencyContact;


        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }



        public IPatientRepository Patients => _patients ??= new PatientRepository(_context);
        public IDoctorRepository Doctors => _doctors ??= new DoctorRepository(_context);
        public IPharmacistRepository Pharmacists => _pharmacists ??= new PharmacistRepository(_context);
        public IHospitalRepository Hospitals => _hospitals ??= new HospitalRepository(_context);
        public IDepartmentRepository Departments => _departments ??= new DepartmentRepository(_context);
        public IPharmacyRepository Pharmacies => _pharmacies ??= new PharmacyRepository(_context);
        public IMedicalRecordRepository MedicalRecords => _medicalRecords ??= new MedicalRecordRepository(_context);
        public IPrescriptionRepository Prescriptions => _prescriptions ??= new PrescriptionRepository(_context);
        public IOTPRepository Otps => _otps ??= new OTPRepository(_context);
        public IAccessAuthorizationRepository AccessAuthorizations => _accessAuthorizations ??= new AccessAuthorizationRepository(_context);
        public IAuditLogRepository AuditLogs => _auditLogs ??= new AuditLogRepository(_context);
        public INotificationRepository Notifications => _notifications ??= new NotificationRepository(_context);
        public IAllergyRepository Allergies => _allergies ??= new AllergyRepository(_context);
        public IChronicDiseaseRepository ChronicDiseases => _chronicDiseases ??= new ChronicDiseaseRepository(_context);
        public IHospitalAdminRepository HospitalAdmin => _hospitalAdmin ??= new HospitalAdminRepository(_context);
        public IPharmacyAdminRepository PharmacyAdmin => _pharmacyAdmin ??= new PharmaicyAdminRepository(_context);
        public IEmergencyContactRepository EmergencyContact => _emergencyContact ??= new EmergencyContactRepository(_context);


        public Task TrackAsAddedAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : PulseID.Domain.Common.BaseEntity
            => _context.AddAsync(entity, cancellationToken).AsTask();
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => _context.SaveChangesAsync(cancellationToken);


        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }




    }
}
