using PulseID.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.DTOs.Patient
{
    public class UpdatePatientDto
    {
        public string PhoneNumber { get; set; } = default!;
        public string? Address { get; set; }
        public Gender Gender { get; set; } 
        public BloodType? BloodType { get; set; }
    }
}
