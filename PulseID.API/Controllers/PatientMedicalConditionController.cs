using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PulseID.Application.DTOs.Allergies;
using PulseID.Application.DTOs.ChronicDiseases;
using PulseID.Application.Interfaces.IServices;
using PulseID.Domain.Entities;

namespace PulseID.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PatientMedicalConditionController : ControllerBase
    {

        readonly IPatientMedicalConditionService _patientMedicalConditionService;

        public PatientMedicalConditionController(IPatientMedicalConditionService patientMedicalConditionService)
        {
            _patientMedicalConditionService = patientMedicalConditionService;
        }

        // ------------- < Allergies > --------------------


        // GET: api/patients/{patientId}/allergies
        [HttpGet("allergies")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Patient,Doctor,HospitalAdmin,SuperAdmin")]
        [EnableRateLimiting("SmallDataLimiting")]
        public async Task<ActionResult<IReadOnlyList<PatientAllergyDto>>> GetAllergies(Guid patientId, CancellationToken cancellationToken)
        {
            var allergies = await _patientMedicalConditionService.GetPatientAllergiesAsync(patientId, cancellationToken);
            return Ok(allergies);
        }


        // POST: api/patients/{patientId}/allergies/{allergyId}
        [HttpPost("allergies")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Patient,Doctor,HospitalAdmin,SuperAdmin")]
        public async Task<ActionResult<PatientAllergyDto>> AddAllergy(Guid patientId, [FromBody] AddPatientAllergyDto dto, CancellationToken cancellationToken)
        {
            var added = await _patientMedicalConditionService.AddAllergyAsync(patientId, dto, cancellationToken);
            return CreatedAtAction(nameof(GetAllergies), new { patientId }, added);
        }

        // DELETE: api/patients/{patientId}/allergies/{allergyId}
        [HttpDelete("allergies/{allergyId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Patient,Doctor,HospitalAdmin,SuperAdmin")]
        public async Task<IActionResult> RemoveAllergy(Guid patientId, Guid allergyId, CancellationToken cancellationToken)
        {
            await _patientMedicalConditionService.RemoveAllergyAsync(patientId, allergyId, cancellationToken);
            return NoContent();
        }



        // ------------- < Chronic Diseases > ------------------


        // GET: api/patients/{patientId}/chronic-diseases
        [HttpGet("chronic-diseases")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Patient,Doctor,HospitalAdmin,SuperAdmin")]
        [EnableRateLimiting("SmallDataLimiting")]
        public async Task<ActionResult<IReadOnlyList<PatientChronicDiseaseDto>>> GetChronicDiseases(Guid patientId, CancellationToken cancellationToken)
        {
            var disease = await _patientMedicalConditionService.GetPatientChronicDiseasesAsync(patientId, cancellationToken);
            return Ok(disease);
        }


        // POST: api/patients/{patientId}/chronic-diseases
        [HttpPost("chronic-diseases")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Patient,Doctor,HospitalAdmin,SuperAdmin")]
        public async Task<ActionResult<PatientChronicDiseaseDto>> AddChronicDisease(Guid patientId, [FromBody] AddPateintChronicDiseaseDto dto, CancellationToken cancellationToken)
        {
            var added = await _patientMedicalConditionService.AddChronicDiseaseAsync(patientId, dto, cancellationToken);
            return CreatedAtAction(nameof(GetChronicDiseases), new { patientId }, added);
        }

        // DELETE: api/patients/{patientId}/chronic-diseases/{chronicDiseaseId}
        [HttpDelete("chronic-diseases/{chronicDiseaseId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Patient,Doctor,HospitalAdmin,SuperAdmin")]
        public async Task<IActionResult> RemoveChronicDisease(Guid patientId, Guid chronicDiseaseId, CancellationToken cancellationToken)
        {
            await _patientMedicalConditionService.RemoveChronicDiseasesAsync(patientId, chronicDiseaseId, cancellationToken);
            return NoContent();
        }

    }
}
