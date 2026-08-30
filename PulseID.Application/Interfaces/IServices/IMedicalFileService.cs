using Microsoft.AspNetCore.Http;
using PulseID.Application.DTOs.MedicalFiles;
using PulseID.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.Interfaces.IServices
{
    public interface IMedicalFileService
    {
        //Task<MedicalFileDto> AddFileAsync(Guid medicalRecordId, AddMedicalFileDto dto, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<MedicalFileDto>> GetByMedicalRecordAsync(Guid medicalRecordId, CancellationToken cancellationToken = default);

        Task<MedicalFileDto> UploadAndAttachAsync(Guid medicalRecordId, IFormFile file, MedicalFileType fileType,
            string? fileDescription = null, CancellationToken cancellationToken = default);

        Task<MedicalFileDownloadDto> DownloadFileAsync(Guid medicalRecordId, Guid fileId, CancellationToken cancellationToken = default);

        Task DeleteFileAsync(Guid medicalRecordId, Guid fileId, CancellationToken cancellationToken = default);
    }
}
