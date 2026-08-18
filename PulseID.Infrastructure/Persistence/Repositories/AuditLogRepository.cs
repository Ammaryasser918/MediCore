using Microsoft.EntityFrameworkCore;
using PulseID.Application.Interfaces.Repositories;
using PulseID.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Infrastructure.Persistence.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        readonly AppDbContext _context;
        public AuditLogRepository(AppDbContext context) => _context = context;

        public async Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken = default)
            => await _context.AuditLogs.AddAsync(auditLog, cancellationToken);

        public async Task<IReadOnlyList<AuditLog>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default)
            => await _context.AuditLogs.Where(a => a.UserId == userId)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<AuditLog>> GetByEntityAsync(string entityName, Guid entityId, CancellationToken cancellationToken = default)
            => await _context.AuditLogs
            .Where(a => a.EntityName == entityName && a.EntityId == entityId)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync(cancellationToken);


        public async Task<IReadOnlyList<AuditLog>> GetByDateRangeAsync(DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken = default)
            => await _context.AuditLogs
            .Where(a => a.Timestamp >= fromUtc && a.Timestamp <= toUtc)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync(cancellationToken);


    }
}
