using PulseID.Application.Common.Interfaces;
using PulseID.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.Interfaces.Repositories
{
    public interface IAccessAuthorizationRepository : IRepository<AccessAuthorization>
    {
        Task<IReadOnlyList<AccessAuthorization>> GetByPatientAsync(Guid patientId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<AccessAuthorization>> GetByRequesterAsync(Guid requesterUserId, CancellationToken cancellationToken = default);
        Task<AccessAuthorization?> GetActiveAuthorizationAsync(Guid patientId, Guid granteeUserId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<AccessAuthorization>> GetPendingForGranteeAsync(Guid granteeUserId, CancellationToken cancellationToken = default);
    }
}
