using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Application.DTOs.ChronicDiseases
{
    public class CreateChronicDiseaseDto
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
    }
}
