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
    internal class EmergencyContactRepository : RepositoryBase<EmergencyContact>, IEmergencyContactRepository
    {
        public EmergencyContactRepository(AppDbContext context) : base(context)
        {
            
        }


        public async Task<IEnumerable<EmergencyContact>> GetByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(ec => ec.PatientId == patientId && !ec.IsDeleted).AsNoTracking().ToListAsync(cancellationToken);
        }

    }
}
