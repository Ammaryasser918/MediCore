using PulseID.Domain.Common;
using PulseID.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Domain.Entities
{
    public class MedicalFile : BaseEntity
    {
        public Guid PatientId { get; private set; }
        public Patient Patient { get; private set; } = default!;

        public Guid MedicalRecordId { get; private set; }
        public MedicalRecord MedicalRecord { get; private set; } = default!;

        public string FileName { get; private set; } = default!;
        public string FilePath { get; private set; } = default!;
        public string ContentType { get; private set; } = default!;
        public MedicalFileType FileType { get; private set; }
        public string? Description { get; private set; }


        MedicalFile() { }

        internal MedicalFile(Guid patientId, Guid medicalRecordId, string fileName, string filePath, string contentType,
            MedicalFileType fileType, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("File name is required", nameof(fileName));
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path is required", nameof(filePath));

            PatientId = patientId;
            MedicalRecordId = medicalRecordId;
            FileName = fileName;
            FilePath = filePath;
            ContentType = contentType;
            FileType = fileType;
            Description = description;

        }





    }
}
