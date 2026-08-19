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
    public class PrescriptionRepository : RepositoryBase<Prescription>, IPrescriptionRepository
    {
        public PrescriptionRepository(AppDbContext context) : base(context)
        {
        }

        public Task<Prescription?> GetWithItemsAsync(Guid prescriptionId, CancellationToken cancellationToken = default)
            => _dbSet.Include(p => p.PrescriptionItems.Where(i => !i.IsDeleted)).FirstOrDefaultAsync(p => p.Id == prescriptionId && !p.IsDeleted, cancellationToken);

        public async Task<IReadOnlyList<Prescription>> GetByMedicalRecordAsync(Guid medicalRecordId, CancellationToken cancellationToken = default)
            => await _dbSet.Where(p => p.MedicalRecordId == medicalRecordId && !p.IsDeleted).ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<Prescription>> GetByStatusAsync(PrescriptionStatus status, CancellationToken cancellationToken = default)
            => await _dbSet.Where(p => p.Status == status && !p.IsDeleted).ToListAsync(cancellationToken);

    }
}
