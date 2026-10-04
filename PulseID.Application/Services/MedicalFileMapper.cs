using PulseID.Application.Common.Interfaces;
using PulseID.Application.DTOs.MedicalFiles;
using PulseID.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.Services
{
    internal static class MedicalFileMapper
    {
        internal static MedicalFileDto ToDto(MedicalFile file, IFileStorageService storage) => new()
        {
            Id = file.Id,
            PatientId = file.PatientId,
            MedicalRecordId = file.MedicalRecordId,
            FileName = file.FileName,
            FileUrl= storage.GetUrl(file.FilePath),
            ContentType = file.ContentType,
            FileType = file.FileType,
            Description = file.Description,
            CreatedAt = file.CreatedAt
        };
    }
}
