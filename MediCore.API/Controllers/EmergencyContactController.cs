using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Identity.Client;
using MediCore.Application.DTOs.EmergencyContact;
using MediCore.Application.Interfaces.IServices;
using MediCore.Application.Services;

namespace MediCore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [EnableRateLimiting("SmallDataLimiter")]
    public class EmergencyContactController : ControllerBase
    {

        private readonly IEmergencyContactService _emergencyContactService;
        public EmergencyContactController(IEmergencyContactService emergencyContactService)
        {
            _emergencyContactService = emergencyContactService;
        }

        [HttpGet]
        [Authorize(Roles = "Patient,Doctor,HospitalAdmin,SuperAdmin")]
        [ProducesResponseType(typeof(IEnumerable<EmergencyContactDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<EmergencyContactDto>>> GetContacts([FromQuery] Guid? patientId, CancellationToken cancellationToken)
        {
            var result = await _emergencyContactService.GetPatientContactsAsync(patientId, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Patient,Doctor,HospitalAdmin,SuperAdmin")]
        [ProducesResponseType(typeof(EmergencyContactDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmergencyContactDto>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _emergencyContactService.GetByIdAsync(id, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Patient,HospitalAdmin,SuperAdmin")]
        [ProducesResponseType(typeof(EmergencyContactDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<EmergencyContactDto>> Create([FromBody] CreateEmergencyContactDto dto,[FromQuery] Guid? patientId, CancellationToken cancellationToken)
        {
            var result = await _emergencyContactService.CreateAsync(dto, patientId, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Patient,HospitalAdmin,SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmergencyContactDto>> Update(Guid id, [FromBody] UpdateEmergencyContactDto dto, CancellationToken cancellationToken)
        {
            var result = await _emergencyContactService.UpdateAsync(id, dto, cancellationToken);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Patient,HospitalAdmin,SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _emergencyContactService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id:guid}/hard-delete")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> HardDelete(Guid id, CancellationToken cancellationToken)
        {
            await _emergencyContactService.HardDeleteAsync(id, cancellationToken);
            return NoContent();
        }


    }
}
