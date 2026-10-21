using MediCore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Application.DTOs.MedicalRecord
{
    public class MedicalRecordListItemDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public Guid HospitalId { get; set; }
        public DateTime VisitDate { get; set; }
        public string Diagnosis { get; set; } = default!;
        public MedicalRecordStatus Status { get; set; }
        public Guid RootRecordId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
