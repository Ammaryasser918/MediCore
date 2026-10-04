using PulseID.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.DTOs.MedicalFiles
{
    public class MedicalFileDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid MedicalRecordId { get; set; }
        public string FileName { get; set; } = default!;
        public string? FileUrl { get; set; } = default!;
        public string ContentType { get; set; } = default!;
        public MedicalFileType FileType { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
