using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Domain.Exceptions
{
    public class BusinessRuleViolationException : DomainException
    {
        public BusinessRuleViolationException(string message) : base(message) { }





    }
}
