using Microsoft.EntityFrameworkCore;
using PulseID.Application.Common.Interfaces;
using PulseID.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Infrastructure.Persistence.Repositories
{
    public class RepositoryBase<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public RepositoryBase(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }


        public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => await _dbSet.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, cancellationToken);

        public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
            => await _dbSet.ToListAsync(cancellationToken);


        public virtual async Task<IReadOnlyList<T>> GetAllActiveAsync(CancellationToken cancellationToken = default)
            => await _dbSet.Where(e => !e.IsDeleted).ToListAsync(cancellationToken);

        public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
            => await _dbSet.AddAsync(entity);

        public virtual void Update(T entity)
            => _dbSet.Update(entity);

        public virtual void Delete(T entity)
            => entity.SoftDelete();


        public virtual async Task<IReadOnlyList<T>> GetDeletedAsync(CancellationToken cancellationToken = default)
            => await _dbSet.Where(e => e.IsDeleted).ToListAsync(cancellationToken);

        public virtual async Task<T?> GetDeletedByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => await _dbSet.FirstOrDefaultAsync(e => e.Id == id && e.IsDeleted, cancellationToken);

        public virtual void Restore(T entity)
        {
            entity.Restore();
            _dbSet.Update(entity);
        }

        public virtual async Task<T?> GetByIdWithDeletedAsync(Guid id, CancellationToken cancellationToken = default)
            => await _dbSet.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        public virtual void HardDelete(T entity)
            => _dbSet.Remove(entity);


    }
}
