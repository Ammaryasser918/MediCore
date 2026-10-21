using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Application.Common.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string msg) : base(msg) { }
        public NotFoundException(string entityName, Guid id) : base($"{entityName} with Id {id} was not Found.") { }
    }
}
