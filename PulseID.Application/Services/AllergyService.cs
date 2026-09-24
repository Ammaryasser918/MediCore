using PulseID.Application.Common.Exceptions;
using PulseID.Application.Common.Interfaces;
using PulseID.Application.DTOs.Allergies;
using PulseID.Application.Interfaces;
using PulseID.Application.Interfaces.IServices;
using PulseID.Domain.Common;
using PulseID.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.Services
{
    public class AllergyService : IAllergyService
    {
        readonly IUnitOfWork _unitOfWork;
        readonly ICurrentUserService _currentUserService;

        public AllergyService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        static AllergyDto ToDto(Allergy allergy) => new()
        {
            Id = allergy.Id,
            Name = allergy.Name,
            Description = allergy.Description
        };

        public async Task<IReadOnlyList<AllergyDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var allergyList = await _unitOfWork.Allergies.GetAllActiveAsync();
            return allergyList.Select(ToDto).ToList();
        }

        public async Task<IReadOnlyList<AllergyDto>> SearchAsync(string term, CancellationToken cancellationToken = default)
        {
            var allergyList = await _unitOfWork.Allergies.SearchByNameAsync(term, cancellationToken);
            return allergyList.Select(ToDto).ToList();
        }

        public async Task<AllergyDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var allergy = await _unitOfWork.Allergies.GetByIdAsync(id, cancellationToken);
            if (allergy is null)
                throw new NotFoundException(nameof(Allergy), id);
            return ToDto(allergy);
        }


        public async Task<AllergyDto> CreateAsync(CreateAllergyDto dto, CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUserService.UserId ?? throw new AuthenticationRequiredException();
            var isAdmin = false;
            if (_currentUserService.Role == AppRoles.SuperAdmin || _currentUserService.Role == AppRoles.HospitalAdmin)
                isAdmin = true;
            if (!isAdmin)
            {
                if (_currentUserService.Role == AppRoles.Doctor)
                {
                    var doctor = await _unitOfWork.Doctors.GetByUserIdAsync(currentUserId, cancellationToken)
                        ?? throw new NotFoundException(nameof(Doctor), currentUserId);

                    if (doctor.Status != Domain.Enums.DoctorStatus.Approved)
                        throw new ForbiddenAccessException("Only an approved doctor can add a new chronic disease to the catalog.");
                }
                else
                {
                    throw new ForbiddenAccessException("Only an approved doctor can add a new chronic disease to the catalog");
                }
            }

            if (await _unitOfWork.Allergies.GetByNameAsync(dto.Name, cancellationToken) is not null)
                throw new DuplicateEntityException($"An allergy named '{dto.Name}' already exists.");

            var allergy = new Allergy(dto.Name, dto.Description);
            await _unitOfWork.Allergies.AddAsync(allergy, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ToDto(allergy);

        }


    }
}
