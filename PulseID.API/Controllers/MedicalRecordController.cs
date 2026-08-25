using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PulseID.Application.DTOs.MedicalRecord;
using PulseID.Application.Interfaces.IServices;

namespace PulseID.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalRecordController : ControllerBase
    {
        readonly IMedicalRecordService _medicalRecordService;

        public MedicalRecordController(IMedicalRecordService medicalRecordService)
        {
            _medicalRecordService = medicalRecordService;
        }


        // POST: api/medicalrecords
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
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<MedicalRecordDto>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var record = await _medicalRecordService.GetByIdAsync(id, cancellationToken);
            return Ok(record);
        }


        // GET: api/medicalrecords/patient/{patientId}?latestOnly=true
        [HttpGet("patient/{patientId:guid}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IReadOnlyList<MedicalRecordListItemDto>>> GetByPatient(Guid patientId, [FromQuery] bool latestOnly = true, CancellationToken cancellationToken = default)
        {
            var records = await _medicalRecordService.GetByPatientAsync(patientId, latestOnly, cancellationToken);
            return Ok(records);
        }


        // GET: api/medicalrecords/{id}/versions 
        [HttpGet("{anyVersionId:guid}/versions")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IReadOnlyList<MedicalRecordVersionDto>>> GetVersionHistory(Guid anyVersionId, CancellationToken cancellationToken)
        {
            var history = await _medicalRecordService.GetVersionHistoryAsync(anyVersionId, cancellationToken);
            return Ok(history);
        }


        // POST: api/medicalrecords/{id}/versions
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
