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
    public class AccessAuthorizationRepository : RepositoryBase<AccessAuthorization>, IAccessAuthorizationRepository
    {
        public AccessAuthorizationRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<AccessAuthorization>> GetByPatientAsync(Guid patientId, CancellationToken cancellationToken = default)
            => await _dbSet.Where(a => a.PatientId == patientId && !a.IsDeleted).ToListAsync(cancellationToken);

        public Task<AccessAuthorization?> GetActiveAuthorizationAsync(Guid patientId, Guid granteeUserId, CancellationToken cancellationToken = default)
            => _dbSet.FirstOrDefaultAsync(a => a.PatientId == patientId &&
                a.GranteeUserId == granteeUserId &&
                a.Status == AuthorizationStatus.Approved &&
                (a.ExpiresAt == null || a.ExpiresAt > DateTime.UtcNow) && !a.IsDeleted,
                cancellationToken);

        public async Task<IReadOnlyList<AccessAuthorization>> GetPendingForGranteeAsync(Guid granteeUserId, CancellationToken cancellationToken = default)
            => await _dbSet.Where(a => a.GranteeUserId == granteeUserId && a.Status == AuthorizationStatus.Pending && !a.IsDeleted)
            .ToListAsync(cancellationToken);


    }
}
