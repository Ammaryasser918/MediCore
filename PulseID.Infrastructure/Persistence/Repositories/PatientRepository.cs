using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PulseID.Application.Interfaces.Repositories;
using PulseID.Domain.Entities;

namespace PulseID.Infrastructure.Persistence.Repositories
{
    public class PatientRepository : RepositoryBase<Patient>, IPatientRepository
    {
        public PatientRepository(AppDbContext context) : base(context) { }


        public Task<Patient?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
            => _dbSet.FirstOrDefaultAsync(p => p.UserId == userId && !p.IsDeleted, cancellationToken);

        public Task<Patient?> GetByNationalIdAsync(string nationalId, CancellationToken cancellationToken = default)
            => _dbSet.FirstOrDefaultAsync(p => p.NationalId == nationalId && !p.IsDeleted, cancellationToken);

        public Task<bool> ExistsByNationalIdAsync(string nationalid, CancellationToken cancellationToken = default)
            => _dbSet.AnyAsync(p => p.NationalId == nationalid && !p.IsDeleted, cancellationToken);

        public Task<Patient?> GetWithFullProfileAsync(Guid patientId, CancellationToken cancellationToken = default)
            => _dbSet.Include(p => p.PatientAllergies)
            .ThenInclude(pa => pa.Allergy)
            .Include(p => p.PatientChronicDiseases)
            .ThenInclude(pcd => pcd.ChronicDisease)
            .Include(p => p.MedicalFiles)
            .Include(p => p.PatientMedications)
            .ThenInclude(pm => pm.Medication)
            .AsSplitQuery() // Avoids Cartesian explosion when loading multiple collection navigations
            .FirstOrDefaultAsync(p => p.Id == patientId && !p.IsDeleted, cancellationToken);

    }
}
