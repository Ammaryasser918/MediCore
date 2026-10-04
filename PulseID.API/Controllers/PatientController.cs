using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.FileProviders;
using PulseID.Application.Common.Interfaces;
using PulseID.Application.DTOs.Doctor;
using PulseID.Application.DTOs.Patient;
using PulseID.Application.Interfaces.IServices;
using PulseID.Application.Services;
using PulseID.Domain.Entities;
using System.Security.Claims;

namespace PulseID.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {

        readonly IPatientService _patientService;
        readonly IAuthorizationService _authorizationService;

        public PatientController(IPatientService patientService, IAuthorizationService authorizationService)
        {
            _patientService = patientService;
            _authorizationService = authorizationService;
        }


        // GET: api/patient
        [Authorize(Roles = "SuperAdmin,HospitalAdmin")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [EnableRateLimiting("BigDataLimiter")]
        public async Task<ActionResult<IReadOnlyList<PatientDto>>> GetAll(CancellationToken cancellationToken)
        {
            var patients = await _patientService.GetAllAsync(cancellationToken);
            return Ok(patients);
        }


        // GET: api/patients/{id}
        [Authorize(Roles = "Patient,Doctor,HospitalAdmin,SuperAdmin")]
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [EnableRateLimiting("SmallDataLimiter")]
        public async Task<ActionResult<PatientDto>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var patient = await _patientService.GetByIdAsync(id, cancellationToken);

            if (patient == null) return NotFound();

            var authResult = await _authorizationService.AuthorizeAsync(User, patient, "PatientOwnerOrAdmin");
            if (!authResult.Succeeded)
                return Forbid();

            return Ok(patient);
        }


        /// <summary>
        /// Retrieves the profile of the currently authenticated patient.
        /// </summary>
        // GET: api/patient/me
        [Authorize(Roles = "Patient")]
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [EnableRateLimiting("SmallDataLimiter")]
        public async Task<ActionResult<PatientDto>> GetCurrentPatient(CancellationToken cancellationToken)
        {
            var Id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(Id, out var userId))
                return Unauthorized("Can't identify the current user.");

            var patient = await _patientService.GetByUserIdAsync(userId, cancellationToken);
            return Ok(patient);
        }

        // GET: api/patients/{nationalId}
        [Authorize(Roles = "Patient,Doctor,HospitalAdmin,SuperAdmin")]
        [HttpGet("by-national-id/{nationalId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [EnableRateLimiting("SmallDataLimiter")]
        public async Task<ActionResult<PatientDto>> GetByNationalId(string nationalId, CancellationToken cancellationToken)
        {
            var patient = await _patientService.GetByNationalIdAsync(nationalId, cancellationToken);

            if (patient == null) return NotFound();

            var authResult = await _authorizationService.AuthorizeAsync(User, patient, "PatientOwnerOrAdmin");
            if (!authResult.Succeeded)
                return Forbid();

            return Ok(patient);
        }


        // POST: api/patients
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,HospitalAdmin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PatientDto>> Create([FromBody] CreatePatientDto dto, CancellationToken cancellationToken)
        {
            var created = await _patientService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/patients/{id}
        [Authorize(Roles = "Patient,SuperAdmin,HospitalAdmin")]
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PatientDto>> Update(Guid id, [FromBody] UpdatePatientDto dto, CancellationToken cancellationToken)
        {

            var existingPatient = await _patientService.GetByIdAsync(id, cancellationToken);
            if (existingPatient is null) return NotFound($"Patient with ID: '{id}' Not Found.");

            var authResult = await _authorizationService.AuthorizeAsync(User, existingPatient, "PatientOwnerOrAdmin");
            if (!authResult.Succeeded)
                return Forbid();

            var updated = await _patientService.UpdateAsync(id, dto, cancellationToken);

            return Ok(updated);
        }


        // DELETE: api/patient/{id}
        [Authorize(Roles = "SuperAdmin,Patient")]
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var existingPatient = await _patientService.GetByIdAsync(id, cancellationToken);
            if (existingPatient is null) return NotFound();

            var authResult = await _authorizationService.AuthorizeAsync(User, existingPatient, "PatientOwnerOrAdmin");
            if (!authResult.Succeeded)
                return Forbid();

            await _patientService.DeleteAsync(id, cancellationToken);
            
            return NoContent();
        }

        [HttpDelete("{id:guid}/hard-delete")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> HardDelete(Guid id, CancellationToken cancellationToken)
        {
            await _patientService.HardDeleteAsync(id, cancellationToken);
            return NoContent();
        }


        [HttpGet("deleted")]
        [Authorize(Roles = "SuperAdmin")]
        [EnableRateLimiting("BigDataLimiter")]
        public async Task<IActionResult> GetDeleted(CancellationToken cancellationToken)
        {
            var result = await _patientService.GetDeletedPatientsAsync(cancellationToken);
            return Ok(result);
        }

        [HttpPost("{id:guid}/restore")]
        [Authorize(Roles = "SuperAdmin")]
        [EnableRateLimiting("SmallDataLimiter")]
        public async Task<IActionResult> Restore(Guid id, CancellationToken cancellationToken)
        {
            await _patientService.RestorePatientAsync(id, cancellationToken);
            return Ok(new { Message = "Record restored successfully." });
        }


        [Authorize(Roles = "Patient,SuperAdmin")]
        [HttpPut("{id:guid}/personal-image")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<PatientDto>> UploadPersonalImage(Guid id, IFormFile file, CancellationToken cancellationToken)
        {
            var existing = await _patientService.GetByIdAsync(id, cancellationToken);
            if (existing is null) return NotFound();
            var auth = await _authorizationService.AuthorizeAsync(User, existing, "PatientOwnerOrAdmin");
            if (!auth.Succeeded) return Forbid();
            return Ok(await _patientService.SetPersonalImageAsync(id, file, cancellationToken));
        }

        [Authorize(Roles = "Patient,SuperAdmin")]
        [HttpPut("{id:guid}/personal-image/remove")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<PatientDto>> RemovePersonalImage(Guid id, IFormFile file, CancellationToken cancellationToken)
        {
            var existing = await _patientService.GetByIdAsync(id, cancellationToken);
            if (existing is null) return NotFound();
            var auth = await _authorizationService.AuthorizeAsync(User, existing, "PatientOwnerOrAdmin");
            if (!auth.Succeeded) return Forbid();
            return Ok(await _patientService.RemovePersonalImageAsync(id, cancellationToken));
        }



    }
}
