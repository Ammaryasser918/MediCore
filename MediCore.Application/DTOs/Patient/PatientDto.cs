using MediCore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Application.DTOs.Patient
{
    public class PatientDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = default!;
        public string NationalId { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; } 
        public Gender Gender { get; set; }
        public BloodType? BloodType { get; set; }
        public string PhoneNumber { get; set; } = default!;
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? PersonalImageUrl { get; set; }
    }
}
