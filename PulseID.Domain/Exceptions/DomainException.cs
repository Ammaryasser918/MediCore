using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Domain.Exceptions
{
    public abstract class DomainException : Exception
    {
        protected DomainException(string msg) : base(msg) { }
    }
}
