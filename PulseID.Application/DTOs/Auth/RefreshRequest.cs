using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.DTOs.Auth
{
    public class RefreshRequest
    {
        public Guid UserId { get; set; }
        public string RefreshToken { get; set; } = default!;
    }
}
