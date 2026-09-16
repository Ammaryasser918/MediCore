using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PulseID.Application.DTOs.MedicalFiles;
using PulseID.Application.Interfaces.IServices;

namespace PulseID.API.Controllers
{
    [Route("api/medicalrecords/{medicalRecordId:guid}/files")]
    [ApiController]
    public class MedicalFilesController : ControllerBase
    {
        readonly IMedicalFileService _medicalFileService;

        public MedicalFilesController(IMedicalFileService medicalFileService)
        {
            _medicalFileService = medicalFileService;
        }

        // GET: api/medicalrecords/{medicalrecordId}/files
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [EnableRateLimiting("BigDataLimiter")]
        public async Task<ActionResult<IReadOnlyList<MedicalFileDto>>> GetAll(Guid medicalRecordId, CancellationToken cancellationToken)
        {
            var files = await _medicalFileService.GetByMedicalRecordAsync(medicalRecordId, cancellationToken);
            return Ok(files);
        }


        // POST: api/medicalrecords/{medicalrecordId}/files
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MedicalFileDto>> Add(Guid medicalRecordId, [FromBody] AddMedicalFileDto dto, CancellationToken cancellationToken)
        {
            var file = await _medicalFileService.AddFileAsync(medicalRecordId, dto, cancellationToken);
            return CreatedAtAction(nameof(GetAll), new { medicalRecordId }, file);
        }


    }
}
