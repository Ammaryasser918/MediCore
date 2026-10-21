using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MediCore.Application.DTOs.PatientMedications;
using MediCore.Application.Interfaces.IServices;
using MediCore.Application.Services;

namespace MediCore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientMedicationController : ControllerBase
    {
        readonly IPatientMedicationService _patientMedicationService;

        public PatientMedicationController(IPatientMedicationService patientMedicationService)
        {
            _patientMedicationService = patientMedicationService;
        }

        [HttpGet("medications")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Patient,Doctor,HospitalAdmin,SuperAdmin")]
        [EnableRateLimiting("SmallDataLimiter")]
        public async Task<ActionResult<IReadOnlyList<PatientMedicationDto>>> GetMedications([FromQuery] Guid? patientId, [FromQuery] bool activeOnly = true, CancellationToken cancellationToken = default)
        {
            var result = await _patientMedicationService.GetPatientMedicationsAsync(patientId, activeOnly, cancellationToken);
            return Ok(result);
        }

        [HttpPost("medications")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Patient,Doctor,HospitalAdmin,SuperAdmin")]
        public async Task<ActionResult<PatientMedicationDto>> AddMedication([FromQuery] Guid? patientId, [FromBody] AddPatientMedicationDto dto, CancellationToken cancellationToken)
        {
            var added = await _patientMedicationService.AddMedicationAsync(patientId, dto, cancellationToken);
            return CreatedAtAction(nameof(GetMedications), new { patientId }, added);
        }

        [HttpPut("medications/{patientMedicationId:guid}/stop")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Patient,Doctor,HospitalAdmin,SuperAdmin")]
        public async Task<IActionResult> StopMedication(
            [FromQuery] Guid? patientId, Guid patientMedicationId,
            [FromBody] StopPatientMedicationDto? dto, CancellationToken cancellationToken)
        {
            await _patientMedicationService.StopMedicationAsync(patientId, patientMedicationId, dto ?? new StopPatientMedicationDto(), cancellationToken);
            return NoContent();
        }

    }
}
