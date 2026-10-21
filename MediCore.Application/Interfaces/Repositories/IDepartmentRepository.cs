using MediCore.Application.Common.Interfaces;
using MediCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Application.Interfaces.Repositories
{
    public interface IDepartmentRepository : IRepository<Department>
    {
        Task<IReadOnlyList<Department>> GetByHospitalAsync(Guid hospitalId, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Department>> GetByHospitalIncludingDeletedAsync(Guid hospitalId, CancellationToken cancellationToken = default);

        Task<Department?> GetWithDoctorsAsync(Guid departmentId, CancellationToken cancellationToken = default);

        Task<bool> ExistsByNameInHospitalAsync(Guid hospitalId, string name, CancellationToken cancellationToken = default);
        
    }
}
