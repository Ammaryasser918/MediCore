using Microsoft.EntityFrameworkCore;
using PulseID.Application.Interfaces.Repositories;
using PulseID.Domain.Entities;
using PulseID.Domain.Enums;

namespace PulseID.Infrastructure.Persistence.Repositories;

public class LabTestRequestRepository : RepositoryBase<LabTestRequest>, ILabTestRequestRepository
{
    public LabTestRequestRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<LabTestRequest>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default)
    => await _dbSet
        .Include(r => r.Patient)
        .Include(r => r.Doctor)
        .Include(r => r.LabTest)
        .Include(r => r.Lab)
        .Where(r => !r.IsDeleted)
        .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<LabTestRequest>> GetByPatientAsync(Guid patientId, CancellationToken cancellationToken = default)
        => await _dbSet
            .Include(r => r.Patient)
            .Include(r => r.Doctor)
            .Include(r => r.LabTest)
            .Include(r => r.Lab)
            .Where(r => r.PatientId == patientId && !r.IsDeleted)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<LabTestRequest>> GetByDoctorAsync(Guid doctorId, CancellationToken cancellationToken = default)
        => await _dbSet
            .Include(r => r.Patient)
            .Include(r => r.Doctor)
            .Include(r => r.LabTest)
            .Include(r => r.Lab)
            .Where(r => r.DoctorId == doctorId && !r.IsDeleted)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<LabTestRequest>> GetByLabAsync(Guid labId, CancellationToken cancellationToken = default)
        => await _dbSet
            .Include(r => r.Patient)
            .Include(r => r.Doctor)
            .Include(r => r.LabTest)
            .Include(r => r.Lab)
            .Where(r => r.LabId == labId && !r.IsDeleted)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<LabTestRequest>> GetByStatusAsync(LabTestRequestStatus status, CancellationToken cancellationToken = default)
        => await _dbSet
            .Include(r => r.Patient)
            .Include(r => r.Doctor)
            .Include(r => r.LabTest)
            .Include(r => r.Lab)
            .Where(r => r.Status == status && !r.IsDeleted)
            .ToListAsync(cancellationToken);

    public Task<LabTestRequest?> GetWithDetailsAsync(Guid requestId, CancellationToken cancellationToken = default)
        => _dbSet
            .Include(r => r.Patient)
            .Include(r => r.Doctor)
            .Include(r => r.LabTest)
            .Include(r => r.Lab)
            .FirstOrDefaultAsync(r => r.Id == requestId && !r.IsDeleted, cancellationToken);
}