using PulseID.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.DTOs.OTP
{
    public class RequestOtpDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public string Email { get; set; } = default!;
        public OtpPurpose Purpose { get; set; } = default!;
        public OtpChannel Channel { get; set; } = default!;
    }
}
