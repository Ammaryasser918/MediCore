using MediCore.Application.Common.Interfaces;
using MediCore.Domain.Entities;
using MediCore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Application.Interfaces.Repositories
{
    public interface IDoctorRepository : IRepository<Doctor>
    {
        Task<Doctor?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<Doctor?> GetBySyndicateNumberAsync(string syndicateNumber, CancellationToken cancellationToken = default);
        Task<Doctor?> GetByNationalIdAsync(string nationalId, CancellationToken cancellationToken = default);
        Task<bool> ExistsByNationalIdAsync(string nationalid, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Doctor>> GetByDepartmentAsync(Guid departmentId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Doctor>> GetByStatusAsync(DoctorStatus status, CancellationToken cancellationToken = default);

    }
}
