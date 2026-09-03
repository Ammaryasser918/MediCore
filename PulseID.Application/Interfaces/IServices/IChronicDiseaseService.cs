using PulseID.Application.DTOs.ChronicDiseases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.Interfaces.IServices
{
    public interface IChronicDiseaseService
    {
        Task<IReadOnlyList<ChronicDiseaseDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ChronicDiseaseDto>> SearchAsync(string term, CancellationToken cancellationToken = default);
        Task<ChronicDiseaseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ChronicDiseaseDto> CreateAsync(CreateChronicDiseaseDto dto, CancellationToken cancellationToken = default);


    }
}
