using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Application.DTOs.LabAdminDto
{
    public class LabAdminDto
    {
        public Guid Id { get; set; } = default!;
        public string Email { get; set; } = string.Empty;
        public Guid LabId { get; set; } 
    }
}
