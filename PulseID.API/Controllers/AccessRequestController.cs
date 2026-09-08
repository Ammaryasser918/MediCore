using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PulseID.Application.DTOs.AccessAuthorization;
using PulseID.Application.Interfaces.IServices;
using System.Security.Claims;

namespace PulseID.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public async Task<ActionResult<AccessRequestDto>> VerifyOtp(Guid id, VerifyAccessOtpDto dto, CancellationToken cancellationToken)
        {
            var result = await _accessRequestService.VerifyAccessOtpAsync(id, dto, cancellationToken);
            return Ok(result);
        }


    }
}
