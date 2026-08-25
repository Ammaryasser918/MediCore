using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.Common.Exceptions
{
    public class AuthenticationRequiredException : Exception
    {
        public AuthenticationRequiredException(string msg = "A valid requester identity is required for this operation.") : base(msg) { }
    }
}
