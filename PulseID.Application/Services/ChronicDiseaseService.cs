using PulseID.Application.Common.Exceptions;
using PulseID.Application.Common.Interfaces;
using PulseID.Application.DTOs.ChronicDiseases;
using PulseID.Application.Interfaces;
using PulseID.Application.Interfaces.IServices;
using PulseID.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.Services
{
    public class ChronicDiseaseService : IChronicDiseaseService
    {
        readonly IUnitOfWork _unitOfWork;
        readonly ICurrentUserService _currentUserService ;

        public ChronicDiseaseService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        static ChronicDiseaseDto ToDto(ChronicDisease disease) => new()
        {
            Id = disease.Id,
            Name = disease.Name,
            Description = disease.Description
        };


        public async Task<IReadOnlyList<ChronicDiseaseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var diseases = await _unitOfWork.ChronicDiseases.GetAllAsync();
            return diseases.OrderBy(d => d.Name).Select(ToDto).ToList();
        }

        public async Task<IReadOnlyList<ChronicDiseaseDto>> SearchAsync(string term, CancellationToken cancellationToken = default)
        {
            var diseases = await _unitOfWork.ChronicDiseases.SearchByNameAsync(term, cancellationToken);
            return diseases.Select(ToDto).ToList();
        }

        public async Task<ChronicDiseaseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var disease = await _unitOfWork.ChronicDiseases.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(ChronicDisease), id);
            return ToDto(disease);
        }

        public async Task<ChronicDiseaseDto> CreateAsync(CreateChronicDiseaseDto dto, CancellationToken cancellationToken = default)
        {

            var currentUserId = _currentUserService.UserId ?? throw new AuthenticationRequiredException();

            var doctor = await _unitOfWork.Doctors.GetByUserIdAsync(currentUserId, cancellationToken)
                ?? throw new NotFoundException(nameof(Doctor), currentUserId);

            if (doctor.Status != Domain.Enums.DoctorStatus.Approved)
                throw new ForbiddenAccessException("Only an approved doctor can add a new chronic disease to the catalog.");

            if (await _unitOfWork.ChronicDiseases.GetByNameAsync(dto.Name, cancellationToken) is not null)
                throw new DuplicateEntityException($"A chronic disease with the name '{dto.Name}' already exists.");

            var disease = new ChronicDisease(dto.Name, dto.Description);
            await _unitOfWork.ChronicDiseases.AddAsync(disease, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return ToDto(disease);
        }


    }
}
