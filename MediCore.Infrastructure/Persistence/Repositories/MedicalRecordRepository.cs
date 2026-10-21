using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using MediCore.Application.Interfaces.Repositories;
using MediCore.Domain.Entities;
using MediCore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Infrastructure.Persistence.Repositories
{
    public class MedicalRecordRepository : RepositoryBase<MedicalRecord>, IMedicalRecordRepository
    {
        public MedicalRecordRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<MedicalRecord>> GetByPatientAsync(Guid patientId, CancellationToken cancellationToken = default)
            => await _dbSet.Where(m => m.PatientId == patientId && !m.IsDeleted).ToListAsync(cancellationToken);

        public Task<MedicalRecord?> GetWithDetailsAsync(Guid medicalRecordId, CancellationToken cancellationToken = default)
            => _dbSet
                .Include(m => m.MedicalFiles)
                .Include(m => m.Prescription).ThenInclude(p => p.PrescriptionItems)
                .FirstOrDefaultAsync(m => m.Id == medicalRecordId && !m.IsDeleted, cancellationToken);

        public Task<MedicalRecord?> GetLatestVersionAsync(Guid rootRecordId, CancellationToken cancellationToken = default)
            => _dbSet.FirstOrDefaultAsync(m => m.RootRecordId == rootRecordId && m.Status == MedicalRecordStatus.Active
                                          && !m.IsDeleted, cancellationToken);


        public async Task<IReadOnlyList<MedicalRecord>> GetByDoctorAsync(Guid doctorId, CancellationToken cancellationToken = default)
            => await _dbSet.Include(r => r.Patient)
            .Where(r => r.DoctorId == doctorId && !r.IsDeleted)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);

    }
}
