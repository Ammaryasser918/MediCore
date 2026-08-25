using PulseID.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.Interfaces.IServices
{
    public interface IAccessValidationService
    {
        Task<bool> HasAccessAsync(Guid patientId, Guid requesterUserId, AccessScope requiredScope, CancellationToken cancellationToken = default);

        Task EnsureAccessAsync(Guid patientId, Guid requesterUserId, AccessScope requiredScope, CancellationToken cancellationToken = default);

    }
}
