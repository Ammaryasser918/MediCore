using PulseID.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.DTOs.Auth
{
    public class RegisterPatientDto
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;

        public string NationalId { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public DateTime DateOfBirth { get; set; } 
        public Gender Gender { get; set; }
        public string PhoneNumber { get; set; } = default!;
    }
}
