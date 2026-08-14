using PulseID.Application.Common.Interfaces;
using PulseID.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.Interfaces.Repositories
{
    public interface IChronicDiseaseRepository : IRepository<ChronicDisease>
    {
        Task<ChronicDisease?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ChronicDisease>> SearchByNameAsync(string term, CancellationToken cancellationToken = default);
    }
}
