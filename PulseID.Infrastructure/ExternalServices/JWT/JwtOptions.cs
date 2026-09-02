using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Infrastructure.ExternalServices.JWT
{
    public class JwtOptions
    {
        public string Secret { get; set; } = default!;
        public string Issuer { get; set; } = "PulseID";
        public string Audience { get; set; } = "PulseID.Clients";
        public int ExpiryMinutes { get; set; } = 60;
    }
}
