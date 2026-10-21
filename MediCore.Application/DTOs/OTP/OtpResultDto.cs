using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Application.DTOs.OTP
{
    public class OtpResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = default!;

        // Temporary while development
        public string Otp { get; set; } = default!;
        public string? RegistrationToken { get; set; } = default!;

    }
}
