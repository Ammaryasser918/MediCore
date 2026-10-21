using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Application.DTOs.Auth
{
    public class VerifyLoginDto
    {
        public string Email { get; set; } = default!;
        public string Otp { get; set; } = default!;
    }
}
