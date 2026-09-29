using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.DTOs.Doctor
{
    public class UpdateDoctorDto
    {
        public string FullName { get; set; } = default!;
        public string NationalId { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string Specialization { get; set; } = default!;
        public DateTime DateOfBirth {  get; set; }
        public string? PersonalImagePath { get; set; }
    }
}
