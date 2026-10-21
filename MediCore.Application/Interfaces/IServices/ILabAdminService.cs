using MediCore.Application.DTOs.LabAdminDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Application.Interfaces.IServices
{
    public interface ILabAdminService
    {
        Task<LabAdminDto> CreateLabAdminAsync(CreateLabAdminDto dto, CancellationToken cancellationToken = default);
        Task<LabAdminDto> GetById(Guid id, CancellationToken cancellationToken = default);
    }
}
