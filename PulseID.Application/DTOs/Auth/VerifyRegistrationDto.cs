using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.DTOs.Auth
{
    public class VerifyRegistrationDto
    {
        public string RegistrationToken { get; set; } = default!;
        public string Otp { get; set; } = default!;
    }
}
