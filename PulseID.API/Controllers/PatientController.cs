using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.FileProviders;
using PulseID.Application.DTOs.Patient;
using PulseID.Application.Interfaces.IServices;
using PulseID.Domain.Entities;

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
        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _patientService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }

    }
}
