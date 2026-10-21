using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Infrastructure.ExternalServices.JWT
{
    public class JwtOptions
    {
        public string Secret { get; set; } = default!;
        public string Issuer { get; set; } = "MediCore";
        public string Audience { get; set; } = "MediCore.Clients";
        public int ExpiryMinutes { get; set; } = 60;
    }
}
