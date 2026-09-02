using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PulseID.Application.Common.Interfaces;
using PulseID.Application.DTOs.MedicalFiles;
using PulseID.Application.Interfaces.IServices;
using PulseID.Domain.Enums;
using System.Reflection;

namespace PulseID.API.Controllers
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

            return Ok(new { filePath });

        }

        [HttpGet("GetFile/{fileName}")]
        public IActionResult GetFile(string fileName)
        {
            var uploadDirectory = @"D:\VS_projects\PulseID\PulseID.API\FileUploads";
            var filePath = Path.Combine(uploadDirectory, fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound(fileName);

            var file = System.IO.File.OpenRead(filePath);
            var mimeType = GetMimeType(filePath);

            return File(file, mimeType);

        }


        [HttpGet("DownloadFile/{fileName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            string folderPath = @"D:\VS_projects\PulseID\PulseID.API\FileUploads";
            string path = Path.Combine(folderPath, fileName);

            if (System.IO.File.Exists(path))
            {
                byte[] bytes = await (System.IO.File.ReadAllBytesAsync(path));
                return File(bytes, "application/octet-stream", fileName);
            }
            else
            {
                return NotFound($"File: '{fileName}' Not Found.");
            }

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

        

        // A helper method to tell browsers (Client) about the data they are handling
        private string GetMimeType(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".pdf" => "application/pdf",
                _ => "application/octet-stream",
            };
        }




    }
}
