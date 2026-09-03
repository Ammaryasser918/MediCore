using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.DTOs.Allergies
{
    public class CreateAllergyDto
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
    }
}
