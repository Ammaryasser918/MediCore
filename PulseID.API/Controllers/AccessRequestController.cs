using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PulseID.Application.DTOs.AccessAuthorization;
using PulseID.Application.Interfaces.IServices;
using System.Security.Claims;


namespace PulseID.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccessRequestController : ControllerBase
    {
        readonly IAccessRequestService _accessRequestService;
        public AccessRequestController(IAccessRequestService accessRequestService)
        {
            _accessRequestService = accessRequestService;
        }


        // POST: api/accessrequest/patients/{patientId}
        [HttpPost("patients/{patientId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [EnableRateLimiting("SmallDataLimiter")]
        [Authorize(Roles = "Doctor,SuperAdmin,HospitalAdmin")]
        public async Task<ActionResult<AccessRequestDto>> RequestAccess(Guid patientId, RequestPatientAccessDto dto, CancellationToken cancellationToken)
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _accessRequestService.RequestAccessAsync(patientId, dto, cancellationToken);
            return Ok(result);
        }

        // POST: api/accessrequest/{id}/verify-otp
        [HttpPost("{id:guid}/verify-otp")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Doctor,SuperAdmin,HospitalAdmin")]
        [EnableRateLimiting("AuthLimiter")]
        public async Task<ActionResult<AccessRequestDto>> VerifyOtp(Guid id, VerifyAccessOtpDto dto, CancellationToken cancellationToken)
        {
            var result = await _accessRequestService.VerifyAccessOtpAsync(id, dto, cancellationToken);
            return Ok(result);
        }

        [HttpGet("patient")]
        [Authorize(Roles = "Patient,SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [EnableRateLimiting("BigDataLimiter")]
        public async Task<ActionResult<IEnumerable<AccessRequestDto>>> GetPatientRequests([FromQuery] bool latestOnly = false, CancellationToken cancellationToken = default)
        {
            var result = await _accessRequestService.GetPatientRequestsAsync(latestOnly, cancellationToken);
            return Ok(result);
        }


        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Patient,Doctor,SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [EnableRateLimiting("SmallDataLimiter")]
        public async Task<ActionResult<AccessRequestDto>> GetRequestById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _accessRequestService.GetRequestByIdAsync(id, cancellationToken);
            return Ok(result);
        }

    }
}
