using PulseID.Application.Common.Interfaces;
using PulseID.Domain.Entities;
using PulseID.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.Interfaces.Repositories
{
    public interface IHospitalRepository : IRepository<Hospital>
    {
        Task<Hospital?> GetWithDepartmentsAsync(Guid hospitalId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Hospital>> GetByStatusAsync(HospitalStatus status, CancellationToken cancellationToken = default);
    }
}
