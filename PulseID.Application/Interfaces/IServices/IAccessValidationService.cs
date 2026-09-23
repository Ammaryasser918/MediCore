using PulseID.Domain.Entities;
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
        Task<bool> HasAccessAsync(Patient patient, Guid requesterUserId, AccessScope requiredScope, CancellationToken cancellationToken = default);

        Task EnsureAccessAsync(Patient patient, Guid requesterUserId, AccessScope requiredScope, CancellationToken cancellationToken = default);

    }
}
