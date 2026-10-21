using MediCore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Application.DTOs.OTP
{
    public class VerifyOtpDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public string Email { get; set; } = default!;
        public string Otp { get; set; } = default!;
        public OtpPurpose Purpose { get; set; }
    }
}
