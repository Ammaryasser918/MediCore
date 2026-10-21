using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Domain.Enums
{
    public enum PrescriptionStatus
    {
        Created = 1,
        Verified = 2,
        PartiallyDispensed = 3,
        Dispensed = 4,
        Cancelled = 5,
        Expired = 6
    }
}
