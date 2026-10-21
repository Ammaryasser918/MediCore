using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Domain.Enums
{
    public enum AuthorizationStatus
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3,
        Revoked = 4,
        Expired = 5
    }
}
