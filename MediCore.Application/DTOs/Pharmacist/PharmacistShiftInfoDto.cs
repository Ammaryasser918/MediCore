using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Application.DTOs.Pharmacist
{
    public class PharmacistShiftInfoDto
    {
        public Guid PharmacistId { get; set; }
        public DateTime? ShiftStartedAt { get; set; }
        public DateTime? ShiftEndedAt { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? WorkingDuration { get; set; }
    }
}
