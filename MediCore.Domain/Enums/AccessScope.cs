using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Domain.Enums
{
    public enum AccessScope
    {
        FullMedicalRecord = 1,
        PrescriptionsOnly = 2,
        LabResultsOnly = 3,
        SpecificFile = 4
    }
}
