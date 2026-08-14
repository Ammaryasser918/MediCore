using PulseID.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.Interfaces.Repositories
{
    public interface IAuditLogRepository
    {
        Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<AuditLog>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<AuditLog>> GetByEntityAsync(string entityName, Guid entityId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<AuditLog>> GetByDateRangeAsync(DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken = default);
    }
}
