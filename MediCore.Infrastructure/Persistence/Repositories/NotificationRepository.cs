using Microsoft.EntityFrameworkCore;
using MediCore.Application.Interfaces.Repositories;
using MediCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Infrastructure.Persistence.Repositories
{
    public class NotificationRepository : RepositoryBase<Notification>, INotificationRepository
    {
        public NotificationRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Notification>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default)
            => await _dbSet.Where(n => n.UserId == userId && !n.IsDeleted)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<Notification>> GetUnreadByUserAsync(Guid userId, CancellationToken cancellationToken = default)
            => await _dbSet.Where(n => n.UserId == userId && !n.IsRead && !n.IsRead && !n.IsDeleted)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);

        public Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default)
            => _dbSet.CountAsync(n => n.UserId == userId && !n.IsRead && !n.IsDeleted, cancellationToken);

    }
}
