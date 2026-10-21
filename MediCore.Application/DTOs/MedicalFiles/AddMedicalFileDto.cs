using MediCore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Application.DTOs.MedicalFiles
{
    public class AddMedicalFileDto
    {
        public string FileName { get; set; } = default!;
        public string FilePath { get; set; } = default!;
        public string ContentType { get; set; } = default!;
        public MedicalFileType FileType { get; set; }
        public string? Description { get; set; }
    }
}
