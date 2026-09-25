using PulseID.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.DTOs.Doctor
{
    public class DoctorDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string SyndicateNumber { get; set; } = default!;
        public string? SyndicateDocumentPath { get; set; }
        public string NationalId { get; set; } = default!;
        public string Specialization { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public DateTime DateOfBirth { get; set; } 
        public int Age { get; set; }
        public string? PersonalImagePath { get; set; }
        public DoctorStatus Status { get; set; }
        public Guid? DepartmentId { get; set; }
        public string? DepartmentName { get; set; } 
        public Guid? ApprovedByHospitalAdminId { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? RejectedByAdminId { get; set; }
        public DateTime? RejectedAt { get; set; }
    }
}
