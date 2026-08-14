using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Domain.Enums
{
    public enum MedicalFileType
    {
        LabResult = 1,
        RadiologyImage = 2,
        PrescriptionScan = 3,
        DischargeSummary = 4,
        Referral = 5,
        Other = 6
    }
}
