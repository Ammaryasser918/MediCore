using MediCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Application.DTOs.AccessAuthorization
{
    public class VerifyAccessOtpDto
    {
        public string Otp { get; set; } = default!;
    }
}
