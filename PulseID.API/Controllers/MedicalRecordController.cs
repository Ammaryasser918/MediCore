using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PulseID.Application.DTOs.MedicalRecord;
using PulseID.Application.Interfaces.IServices;

namespace PulseID.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalRecordController : ControllerBase
    {
        readonly IMedicalRecordService _medicalRecordService;
        readonly IAuthorizationService _authorizationService;
        readonly IPatientService _patientService;

        public MedicalRecordController(IMedicalRecordService medicalRecordService, IAuthorizationService authorizationService, IPatientService patientService)
        {
            _medicalRecordService = medicalRecordService;
            _authorizationService = authorizationService;
            _patientService = patientService;
        }


        // POST: api/medicalrecords
        [Authorize(Roles = "Doctor,HospitalAdmin,SuperAdmin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<MedicalRecordDto>> Create([FromBody] CreateMedicalRecordDto dto, CancellationToken cancellationToken)
        {
            var created = await _medicalRecordService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }


        // GET: api/medicalrecords/{id}
        [Authorize(Roles = "Patient,Doctor,HospitalAdmin,SuperAdmin")]
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [EnableRateLimiting("SmallDataLimiter")]
        public async Task<ActionResult<MedicalRecordDto>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var record = await _medicalRecordService.GetByIdAsync(id, cancellationToken);
            if (record is null) return NotFound();
            var authResult = await _authorizationService.AuthorizeAsync(User, record, "MedicalRecordOwnerOrAdmin");
            if (!authResult.Succeeded) return Forbid();
                return Ok(record);
        }


        // GET: api/medicalrecords/patient/{patientId}?latestOnly=true
        [Authorize(Roles = "Patient,Doctor,HospitalAdmin,SuperAdmin")]
        [HttpGet("patient/{patientId:guid}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [EnableRateLimiting("BigDataLimiter")]
        public async Task<ActionResult<IReadOnlyList<MedicalRecordListItemDto>>> GetByPatient(Guid patientId, [FromQuery] bool latestOnly = true, CancellationToken cancellationToken = default)
        {
            var patient = await _patientService.GetByIdAsync(patientId, cancellationToken);
            if (patient is null) return NotFound("Patient not found.");

            var authResult = await _authorizationService.AuthorizeAsync(User, patient, "MedicalRecordOwnerOrAdmin");
            if (!authResult.Succeeded) return Forbid();

            var records = await _medicalRecordService.GetByPatientAsync(patientId, latestOnly, cancellationToken);
            return Ok(records);
        }


        // GET: api/medicalrecords/{id}/versions 
        [Authorize(Roles = "Patient,Doctor,HospitalAdmin,SuperAdmin")]
        [HttpGet("{anyVersionId:guid}/versions")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [EnableRateLimiting("BigDataLimiter")]
        public async Task<ActionResult<IReadOnlyList<MedicalRecordVersionDto>>> GetVersionHistory(Guid anyVersionId, CancellationToken cancellationToken)
        {

            var record = await _medicalRecordService.GetByIdAsync(anyVersionId, cancellationToken);
            if (record is null) return NotFound("Medical record not found.");


            var authResult = await _authorizationService.AuthorizeAsync(User, record, "MedicalRecordOwnerOrAdmin");
            if (!authResult.Succeeded) return Forbid();

            var history = await _medicalRecordService.GetVersionHistoryAsync(anyVersionId, cancellationToken);
            return Ok(history);
        }


        // POST: api/medicalrecords/{id}/versions
        [Authorize(Roles = "Doctor,HospitalAdmin,SuperAdmin")]
        [HttpPost("{currentVersionId:guid}/versions")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<MedicalRecordDto>> CreateNewVersion(Guid currentVersionId, [FromBody] CreateMedicalRecordVersionDto dto, CancellationToken cancellationToken)
        {
            var newVersion = await _medicalRecordService.CreateNewVersionAsync(currentVersionId, dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = newVersion.Id }, newVersion);
        }


    }
}
