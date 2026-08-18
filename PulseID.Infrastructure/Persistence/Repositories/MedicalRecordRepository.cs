using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using PulseID.Application.Interfaces.Repositories;
using PulseID.Domain.Entities;
using PulseID.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Infrastructure.Persistence.Repositories
{
    public class MedicalRecordRepository : RepositoryBase<MedicalRecord>, IMedicalRecordRepository
    {
        public MedicalRecordRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<MedicalRecord>> GetByPatientAsync(Guid patientId, CancellationToken cancellationToken = default)
            => await _dbSet.Where(m => m.PatientId == patientId).ToListAsync(cancellationToken);

        public Task<MedicalRecord?> GetWithDetailsAsync(Guid medicalRecordId, CancellationToken cancellationToken = default)
            => _dbSet
                .Include(m => m.MedicalFiles)
                .Include(m => m.Prescription).ThenInclude(p => p.PrescriptionItems)
                .FirstOrDefaultAsync(m => m.Id == medicalRecordId, cancellationToken);

        public Task<MedicalRecord?> GetLatestVersionAsync(Guid rootRecrdId, CancellationToken cancellationToken = default)
            => _dbSet.FirstOrDefaultAsync(m => m.RootRecordId == rootRecrdId && m.Status == MedicalRecordStatus.Active
                                          && !m.IsDeleted, cancellationToken);


        public async Task<IReadOnlyList<MedicalRecord>> GetByDoctorAsync(Guid doctorId, CancellationToken cancellationToken = default)
            => await _dbSet.Where(m => m.DoctorId == doctorId).ToListAsync(cancellationToken);

    }
}
