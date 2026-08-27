using PulseID.Application.DTOs.MedicalFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.Interfaces.IServices
{
    public interface IMedicalFileService
    {
        Task<MedicalFileDto> AddFileAsync(Guid medicalRecordId, AddMedicalFileDto dto, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<MedicalFileDto>> GetByMedicalRecordAsync(Guid medicalRecordId, CancellationToken cancellationToken = default);
    }
}
