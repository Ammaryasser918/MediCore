using PulseID.Application.Common.Interfaces;
using PulseID.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.Interfaces.Repositories
{
    public interface INotificationRepository : IRepository<Notification>
    {
        Task<IReadOnlyList<Notification>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Notification>> GetUnreadByUserAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
