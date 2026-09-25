using PulseID.Application.DTOs.AccessAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.Interfaces.IServices
{
    public interface IAccessRequestService
    {
        Task<AccessRequestDto> RequestAccessAsync(Guid patientId, RequestPatientAccessDto dto, CancellationToken cancellationToken = default);
        Task<AccessRequestDto> VerifyAccessOtpAsync(Guid accessRequestId, VerifyAccessOtpDto dto, CancellationToken cancellationToken = default);

        Task<IEnumerable<AccessRequestDto>> GetPatientRequestsAsync(bool latestOnly = false, CancellationToken cancellationToken = default);
        Task<AccessRequestDto> GetRequestByIdAsync(Guid requestId, CancellationToken cancellationToken = default);

        Task RejectAccessRequestAsync(Guid requestId, CancellationToken cancellationToken = default);

        Task ApproveAccessRequestAsync(Guid requestId, CancellationToken cancellationToken = default);

        Task RevokeAccessRequestAsync(Guid accessRequestId, CancellationToken cancellationToken = default);

    }
}
