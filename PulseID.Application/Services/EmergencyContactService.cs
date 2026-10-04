using PulseID.Application.Common.Exceptions;
using PulseID.Application.Common.Interfaces;
using PulseID.Application.DTOs.EmergencyContact;
using PulseID.Application.Interfaces;
using PulseID.Application.Interfaces.IServices;
using PulseID.Domain.Common;
using PulseID.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.Services
{
    public class EmergencyContactService : IEmergencyContactService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public EmergencyContactService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }


        public async Task<IEnumerable<EmergencyContactDto>> GetPatientContactsAsync(Guid? patientId, CancellationToken cancellationToken = default)
        {
            var resolvedPatientId = await ResolvePatientIdAsync(patientId, cancellationToken);
            //await ValidateAccessAsync(resolvedPatientId, cancellationToken);
            var contacts = await _unitOfWork.EmergencyContact.GetByPatientIdAsync(resolvedPatientId, cancellationToken);
            return contacts.Select(ToDto);
        }

        public async Task<EmergencyContactDto> GetByIdAsync(Guid Id, CancellationToken cancellationToken = default)
        {
            var contact = await _unitOfWork.EmergencyContact.GetByIdAsync(Id, cancellationToken);
            if (contact is null) throw new NotFoundException("Emergency contact not found.");

            //await ValidateAccessAsync(contact.PatientId, cancellationToken);
            return ToDto(contact);
        }

        public async Task<EmergencyContactDto> CreateAsync(CreateEmergencyContactDto dto, Guid? patientId, CancellationToken cancellationToken = default)
        {
            var resolvedPatientId = await ResolvePatientIdAsync(patientId, cancellationToken);
            await ValidateAccessAsync(resolvedPatientId, cancellationToken);
            var contact = new EmergencyContact(
                resolvedPatientId,
                dto.FullName,
                dto.Relationship,
                dto.PhoneNumber,
                dto.Address
                );
            await _unitOfWork.EmergencyContact.AddAsync(contact, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ToDto(contact);

        }

        public async Task<EmergencyContactDto> UpdateAsync(Guid id, UpdateEmergencyContactDto dto,  CancellationToken cancellationToken = default)
        {
            var contact = await _unitOfWork.EmergencyContact.GetByIdAsync(id, cancellationToken);
            if (contact is null)
                throw new NotFoundException("Emergency contact not found.");
            await ValidateAccessAsync(contact.PatientId, cancellationToken);

            contact.UpdateDetails(dto.FullName, dto.Relationship, dto.PhoneNumber, dto.Address);
            _unitOfWork.EmergencyContact.Update(contact);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return ToDto(contact);
        }
       
        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var contact = await _unitOfWork.EmergencyContact.GetByIdAsync(id, cancellationToken);
            if (contact is null)
                throw new NotFoundException("Emergency Contact not found.");

            await ValidateAccessAsync(contact.PatientId, cancellationToken);

            _unitOfWork.EmergencyContact.Delete(contact);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }


        public async Task HardDeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var contact = await _unitOfWork.EmergencyContact.GetByIdWithDeletedAsync(id, cancellationToken);
            if (contact is null) throw new NotFoundException("Emergency contact not found.");

            try
            {
                _unitOfWork.EmergencyContact.HardDelete(contact);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (Exception)
            {
                throw new Exception("Failed to delete emergency contact.");
            }
        }


        // Helpers
        Guid RequireCurrentUserId()
            => _currentUserService.UserId ?? throw new AuthenticationRequiredException();

        async Task<Guid> ResolvePatientIdAsync(Guid? patientId, CancellationToken cancellationToken)
        {
            if (patientId.HasValue && patientId.Value != Guid.Empty)
                return patientId.Value;
            var currentUserId = RequireCurrentUserId();
            var role = _currentUserService.Role;

            if (string.Equals(role, AppRoles.Patient, StringComparison.OrdinalIgnoreCase))
            {
                var patient = await _unitOfWork.Patients.GetByUserIdAsync(currentUserId, cancellationToken)
                    ?? throw new NotFoundException($"Patient profile not found for user ID: '{currentUserId}'.");
                return patient.Id;
            }

            throw new ArgumentException("PatientId query parameter is required for non-patient callers.");

        }

        private async Task ValidateAccessAsync(Guid patientId, CancellationToken cancellationToken)
        {
            if (_currentUserService.Role == AppRoles.SuperAdmin || _currentUserService.Role == AppRoles.HospitalAdmin)
                return;

            if (_currentUserService.Role == AppRoles.Patient)
            {
                var patient = await _unitOfWork.Patients.GetByIdAsync(patientId, cancellationToken);
                if (patient != null && (patient.UserId == _currentUserService.UserId))
                    return;
            }

            throw new ForbiddenAccessException("You do not have permission to access or modify this emergency contact.");
        }

        static EmergencyContactDto ToDto(EmergencyContact contact)
        {
            return new EmergencyContactDto
            {
                Id = contact.Id,
                PatientId = contact.PatientId,
                FullName = contact.FullName,
                Relationship = contact.Relationship,
                PhoneNumber = contact.PhoneNumber,
                Address = contact.Address
            };

        }
    }
}
