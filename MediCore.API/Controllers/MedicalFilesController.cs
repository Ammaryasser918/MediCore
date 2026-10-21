using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MediCore.Application.Common.Interfaces;
using Microsoft.AspNetCore.RateLimiting;
using MediCore.Application.DTOs.MedicalFiles;
using MediCore.Application.Interfaces.IServices;
using MediCore.Domain.Enums;
using System.Reflection;

namespace MediCore.API.Controllers
{
    [Route("api/medicalrecords/{medicalRecordId:guid}/files")]
    [ApiController]
    public class MedicalFilesController : ControllerBase
    {
        readonly IMedicalFileService _medicalFileService;
        readonly IFileStorageService _uploadFileService;
        

        public MedicalFilesController(IMedicalFileService medicalFileService, IFileStorageService uploadFileService)
        {
            _medicalFileService = medicalFileService;
            _uploadFileService = uploadFileService;
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


        //// POST: api/medicalrecords/{medicalrecordId}/files
        //[HttpPost]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public async Task<ActionResult<MedicalFileDto>> Add(Guid medicalRecordId, [FromBody] AddMedicalFileDto dto, CancellationToken cancellationToken)
        //{
        //    var file = await _medicalFileService.AddFileAsync(medicalRecordId, dto, cancellationToken);
        //    return CreatedAtAction(nameof(GetAll), new { medicalRecordId }, file);
        //}


        [HttpPost("Upload")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UploadFile(Guid medicalRecordId, IFormFile file, MedicalFileType medicalFileType,
            string? fileDescription = null, CancellationToken cancellationToken = default)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var filePath = await _medicalFileService.UploadAndAttachAsync(medicalRecordId, file, medicalFileType, fileDescription, cancellationToken);

            return Ok(filePath);

        }

        


        

        [HttpGet("DownloadFileAsync/{fileName}")]
        public async Task<IActionResult> Download(Guid medicalRecordId, Guid fileId, CancellationToken cancellationToken)
        {
            var download = await _medicalFileService.DownloadFileAsync(medicalRecordId, fileId, cancellationToken);
            return File(download.Content, download.ContentType, download.FileName);
        }

        [HttpDelete("DeleteFile/{fileName}")]
        [HttpDelete("{fileId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteFileAsync(Guid medicalRecordId, Guid fileId, CancellationToken cancellationToken)
        {
            await _medicalFileService.DeleteFileAsync(medicalRecordId, fileId, cancellationToken);
            return NoContent();
        }





    }
}
