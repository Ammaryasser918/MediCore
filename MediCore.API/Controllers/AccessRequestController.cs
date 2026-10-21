using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MediCore.Application.DTOs.AccessAuthorization;
using MediCore.Application.Interfaces.IServices;
using System.Security.Claims;


namespace MediCore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccessRequestController : ControllerBase
    {
        readonly IAccessRequestService _accessRequestService;
        readonly IAuthorizationService _authorizationService;
        public AccessRequestController(IAccessRequestService accessRequestService, IAuthorizationService authorizationService)
        {
            _accessRequestService = accessRequestService;
            _authorizationService = authorizationService;
        }


        // POST: api/accessrequest/patients/{patientId}
        [HttpPost("patients/{patientId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [EnableRateLimiting("SmallDataLimiter")]
        [Authorize(Roles = "Doctor,SuperAdmin,HospitalAdmin,Pharmacist,PharmacyAdmin,LabTechnician,LabAdmin")]
        public async Task<ActionResult<AccessRequestDto>> RequestAccess(Guid patientId, RequestPatientAccessDto dto, CancellationToken cancellationToken)
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _accessRequestService.RequestAccessAsync(patientId, dto, cancellationToken);
            //var auth = await _authorizationService.AuthorizeAsync(User, result, "AccessRequestOwnerOrAdmin");
            //if (!auth.Succeeded)
            //{
            //    return Forbid();
            //}
            return Ok(result);
        }

        // POST: api/accessrequest/{id}/verify-otp
        [HttpPost("{id:guid}/verify-otp")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Doctor,SuperAdmin,HospitalAdmin,Pharmacist,PharmacyAdmin,LabTechnician,LabAdmin")]
        [EnableRateLimiting("AuthLimiter")]
        public async Task<ActionResult<AccessRequestDto>> VerifyOtp(Guid id, VerifyAccessOtpDto dto, CancellationToken cancellationToken)
        {
            var result = await _accessRequestService.VerifyAccessOtpAsync(id, dto, cancellationToken);
            var auth = await _authorizationService.AuthorizeAsync(User, result, "AccessRequestOwnerOrAdmin");
            if (!auth.Succeeded)
            {
                return Forbid();
            }
            return Ok(result);
        }

        [HttpGet("patient")]
        [Authorize(Roles = "Patient,SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [EnableRateLimiting("BigDataLimiter")]
        public async Task<ActionResult<IEnumerable<AccessRequestDto>>> GetPatientRequests([FromQuery] Guid? patientId, [FromQuery] bool latestOnly = false, CancellationToken cancellationToken = default)
        {
            var result = await _accessRequestService.GetPatientRequestsAsync(patientId, latestOnly, cancellationToken);
            var authorizedRequests = new List<AccessRequestDto>();
            foreach (var request in result)
            {
                var auth = await _authorizationService.AuthorizeAsync(User, request, "AccessRequestOwnerOrAdmin");
                authorizedRequests.Add(request);
            }
            return Ok(authorizedRequests);
        }

        [HttpGet("requester")]
        [Authorize(Roles = "SuperAdmin,Doctor,Pharmacist,PharmacyAdmin,LabTechnician,LabAdmin,HospitalAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [EnableRateLimiting("BigDataLimiter")]
        public async Task<ActionResult<IEnumerable<AccessRequestDto>>> GetRequesterRequests([FromQuery] Guid? requesterId, [FromQuery] bool latestOnly = false, CancellationToken cancellationToken = default)
        {

            var result = await _accessRequestService.GetRequestsByRequesterAsync(requesterId, latestOnly, cancellationToken);
            var authorizedRequests = new List<AccessRequestDto>();
            foreach (var request in result)
            {
                var auth = await _authorizationService.AuthorizeAsync(User, request, "AccessRequestOwnerOrAdmin");
                authorizedRequests.Add(request);
            }
            return Ok(authorizedRequests);
        }


        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Patient,Doctor,SuperAdmin,Pharmacist,PharmacyAdmin,LabTechnician,LabAdmin,HospitalAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [EnableRateLimiting("SmallDataLimiter")]
        public async Task<ActionResult<AccessRequestDto>> GetRequestById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _accessRequestService.GetRequestByIdAsync(id, cancellationToken);
            if(result is null)
                return NotFound();
            var auth = await _authorizationService.AuthorizeAsync(User, result, "AccessRequestOwnerOrAdmin");
            if (!auth.Succeeded)
            {
                return Forbid();
            }
            return Ok(result);
        }

        [HttpPut("{id:guid}/reject")]
        [Authorize(Roles = "Patient,SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RejectRequest(Guid id, CancellationToken cancellationToken)
        {
            var request = await _accessRequestService.GetRequestByIdAsync(id, cancellationToken);
            if (request is null)
                return NotFound();
            var auth = await _authorizationService.AuthorizeAsync(User, request, "AccessRequestOwnerOrAdmin");
            if (!auth.Succeeded)
            {
                return Forbid();
            }
            await _accessRequestService.RejectAccessRequestAsync(id, cancellationToken);
            return Ok(new { message = "Access request rejected successfully." });
        }

        [HttpPut("{id:guid}/approve")]
        [Authorize(Roles = "Patient,SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ApproveRequest(Guid id, CancellationToken cancellationToken)
        {
            var request = await _accessRequestService.GetRequestByIdAsync(id, cancellationToken);
            if (request is null)
                return NotFound();
            var auth = await _authorizationService.AuthorizeAsync(User, request, "AccessRequestOwnerOrAdmin");
            if (!auth.Succeeded)
            {
                return Forbid();
            }

            await _accessRequestService.ApproveAccessRequestAsync(id, cancellationToken);
            return Ok(new { message = "Access request approved successfully." });
        }


        [HttpPut("{id:guid}/revoke")]
        [Authorize(Roles = "Patient,SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RevokeRequest(Guid id, CancellationToken cancellationToken)
        {
            var request = await _accessRequestService.GetRequestByIdAsync(id, cancellationToken);
            if (request is null)
                return NotFound();
            var auth = await _authorizationService.AuthorizeAsync(User, request, "AccessRequestOwnerOrAdmin");
            if (!auth.Succeeded)
            {
                return Forbid();
            }
            await _accessRequestService.RevokeAccessRequestAsync(id, cancellationToken);
            return Ok(new { message = "Access authorization revoked successfully." });
        }

    }
}
