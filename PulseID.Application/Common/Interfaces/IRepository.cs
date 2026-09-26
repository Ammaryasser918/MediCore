using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PulseID.Domain.Common;

namespace PulseID.Application.Common.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<IReadOnlyList<T>> GetAllActiveAsync (CancellationToken cancellationToken = default);

        Task AddAsync(T entity, CancellationToken cancellationToken = default);

        void Update(T entity);

        void Delete(T entity);

        Task<IReadOnlyList<T>> GetDeletedAsync(CancellationToken cancellationToken = default);
        Task<T?> GetDeletedByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<T?> GetByIdWithDeletedAsync(Guid id, CancellationToken cancellationToken = default);
        void Restore(T entity);
        void HardDelete(T entity);

    }
}
