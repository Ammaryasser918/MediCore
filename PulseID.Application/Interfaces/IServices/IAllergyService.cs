using PulseID.Application.DTOs.Allergies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.Interfaces.IServices
{
    public interface IAllergyService
    {
        Task<IReadOnlyList<AllergyDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<AllergyDto>> SearchAsync(string term, CancellationToken cancellationToken = default);
        Task<AllergyDto> CreateAsync(CreateAllergyDto dto, CancellationToken cancellationToken = default);
        Task<AllergyDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
