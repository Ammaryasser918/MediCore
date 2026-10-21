using MediCore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Application.DTOs.AccessAuthorization
{
    public class RequestPatientAccessDto
    {
        public string Purpose { get; set; } = default!;
        public int RequesteDurationMinutes { get; set; } = 60;
        public AccessScope Scope { get; set; } = AccessScope.FullMedicalRecord;
    }
}
