using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.DTOs.EmergencyContact
{
    public class UpdateEmergencyContactDto
    {
        [Required(ErrorMessage = "Full Name is required.")]
        [StringLength(250)]
        public string FullName { get; set; } = default!;

        [Required(ErrorMessage = "Relationship is required.")]
        [StringLength(200)]
        public string Relationship { get; set; } = default!;

        [Required(ErrorMessage = "Phone Number is required.")]
        [Phone]
        public string PhoneNumber { get; set; } = default!;

        [StringLength(800)]
        public string? Address { get; set; }
    }
}
