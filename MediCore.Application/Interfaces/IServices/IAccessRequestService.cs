using MediCore.Application.DTOs.AccessAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Application.Interfaces.IServices
{
    public interface IAccessRequestService
    {
        Task<AccessRequestDto> RequestAccessAsync(Guid patientId, RequestPatientAccessDto dto, CancellationToken cancellationToken = default);
        Task<AccessRequestDto> VerifyAccessOtpAsync(Guid accessRequestId, VerifyAccessOtpDto dto, CancellationToken cancellationToken = default);

        Task<IEnumerable<AccessRequestDto>> GetPatientRequestsAsync(Guid? patientId, bool latestOnly = false, CancellationToken cancellationToken = default);
        Task<IEnumerable<AccessRequestDto>> GetRequestsByRequesterAsync(Guid? requesterId, bool latestOnly = false, CancellationToken cancellationToken = default);
        Task<AccessRequestDto> GetRequestByIdAsync(Guid requestId, CancellationToken cancellationToken = default);

        Task RejectAccessRequestAsync(Guid requestId, CancellationToken cancellationToken = default);

        Task ApproveAccessRequestAsync(Guid requestId, CancellationToken cancellationToken = default);

        Task RevokeAccessRequestAsync(Guid accessRequestId, CancellationToken cancellationToken = default);

    }
}
