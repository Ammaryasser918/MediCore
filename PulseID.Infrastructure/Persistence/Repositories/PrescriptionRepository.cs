using Microsoft.EntityFrameworkCore;
using PulseID.Application.Interfaces.Repositories;
using PulseID.Domain.Entities;
using PulseID.Domain.Enums;

namespace PulseID.Infrastructure.Persistence.Repositories;

public class PrescriptionRepository : RepositoryBase<Prescription>, IPrescriptionRepository
{
    public PrescriptionRepository(AppDbContext context) : base(context)
    {
    }

    public override async Task<Prescription?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.MedicalRecord)
            .Include(p => p.PrescriptionItems.Where(i => !i.IsDeleted))
                .ThenInclude(i => i.Medication)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, cancellationToken);
    }

    public Task<Prescription?> GetWithItemsAsync(Guid prescriptionId, CancellationToken cancellationToken = default)
        => _dbSet
            .Include(p => p.MedicalRecord)
            .Include(p => p.PrescriptionItems.Where(i => !i.IsDeleted))
                .ThenInclude(i => i.Medication)
            .FirstOrDefaultAsync(p => p.Id == prescriptionId && !p.IsDeleted, cancellationToken);

    public async Task<IReadOnlyList<Prescription>> GetByMedicalRecordAsync(Guid medicalRecordId, CancellationToken cancellationToken = default)
        => await _dbSet
            .Where(p => p.MedicalRecordId == medicalRecordId && !p.IsDeleted)
            .Include(p => p.MedicalRecord)
            .Include(p => p.PrescriptionItems.Where(i => !i.IsDeleted))
                .ThenInclude(i => i.Medication)
            .OrderByDescending(p => p.IssueDate)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Prescription>> GetByStatusAsync(PrescriptionStatus status, CancellationToken cancellationToken = default)
        => await _dbSet
            .Where(p => p.Status == status && !p.IsDeleted)
            .Include(p => p.MedicalRecord)
            .Include(p => p.PrescriptionItems.Where(i => !i.IsDeleted))
                .ThenInclude(i => i.Medication)
            .OrderByDescending(p => p.IssueDate)
            .ToListAsync(cancellationToken);
}