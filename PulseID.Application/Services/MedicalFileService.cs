using PulseID.Application.Common.Exceptions;
using PulseID.Application.Common.Interfaces;
using PulseID.Application.DTOs.MedicalFiles;
using PulseID.Application.Interfaces;
using PulseID.Application.Interfaces.IServices;
using PulseID.Domain.Entities;
using PulseID.Domain.Enums;
using PulseID.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using PulseID.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;


namespace PulseID.Application.Services
{
    public class MedicalFileService : IMedicalFileService
    {
        readonly IUnitOfWork _unitOfWork;
        readonly IAccessValidationService _accessValidationService;
        readonly ICurrentUserService _currentUser;
        readonly IFileStorageService _uploadFileService;

        public MedicalFileService(IUnitOfWork unitOfWork, IAccessValidationService accessValidationService,
            ICurrentUserService currentUser, IFileStorageService uploadFileService)
        {
            _unitOfWork = unitOfWork;
            _accessValidationService = accessValidationService;
            _currentUser = currentUser;
            _uploadFileService = uploadFileService;
        }


        Guid RequireCurrentUserId()
            => _currentUser.UserId ?? throw new AuthenticationRequiredException();


        //public async Task<MedicalFileDto> AddFileAsync(Guid medicalRecordId, AddMedicalFileDto dto, CancellationToken cancellationToken = default)
        //{
        //    var record = await _unitOfWork.MedicalRecords.GetByIdAsync(medicalRecordId, cancellationToken)
        //        ?? throw new NotFoundException(nameof(MedicalRecord), medicalRecordId);

        //    if (record.Status == MedicalRecordStatus.Superseded)
        //    {
        //        throw new BusinessRuleViolationException("Can't attach files to a supereded (old) version of a medical record. Attach to the latest version instead.");
        //    }


        //    var currentUserId = RequireCurrentUserId();

        //    await _accessValidationService.EnsureAccessAsync(record.PatientId, currentUserId, AccessScope.FullMedicalRecord, cancellationToken);

        //    var patient = await _unitOfWork.Patients.GetByIdAsync(record.PatientId, cancellationToken) ?? throw new NotFoundException(nameof(Patient), record.PatientId);

        //    var file = patient.AddMedicalFile(record.Id, dto.FileName, dto.FilePath, dto.ContentType, dto.FileType, dto.Description);

        //    await _unitOfWork.TrackAsAddedAsync(file, cancellationToken);

        //    await _unitOfWork.SaveChangesAsync(cancellationToken);
        //    return MedicalRecordService.ToFileDto(file);

        //}

        public async Task<IReadOnlyList<MedicalFileDto>> GetByMedicalRecordAsync(Guid medicalRecordId, CancellationToken cancellationToken = default)
        {
            var record = await _unitOfWork.MedicalRecords.GetWithDetailsAsync(medicalRecordId, cancellationToken)
                ?? throw new NotFoundException(nameof(MedicalRecord), medicalRecordId);

            var currentUserId = RequireCurrentUserId();

            await _accessValidationService.EnsureAccessAsync(record.PatientId, currentUserId, AccessScope.FullMedicalRecord, cancellationToken);

            return record.MedicalFiles.Select(MedicalRecordService.ToFileDto).ToList();

        }

        //public async Task<string?> SaveToUploadsFolder(IFormFile file, CancellationToken cancellationToken = default)
        //{
        //    var uploadDirectory = @"D:\VS_projects\PulseID\PulseID.API\FileUploads";

        //    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        //    var filePath = Path.Combine(uploadDirectory, fileName);

        //    if (!Directory.Exists(uploadDirectory))
        //        Directory.CreateDirectory(uploadDirectory);

        //    using (var stream = new FileStream(filePath, FileMode.Create))
        //    {
        //        await file.CopyToAsync(stream);
        //    }
        //    return filePath;
        //}

        public async Task<MedicalFileDto> UploadAndAttachAsync(Guid medicalRecordId, IFormFile file, MedicalFileType fileType, 
            string? fileDescription = null, CancellationToken cancellationToken = default)
        {
            var record = await _unitOfWork.MedicalRecords.GetByIdAsync(medicalRecordId, cancellationToken) 
                ?? throw new NotFoundException(nameof(MedicalRecord), medicalRecordId);

            var currentUserId = RequireCurrentUserId();
            await _accessValidationService.EnsureAccessAsync(record.PatientId, currentUserId, AccessScope.FullMedicalRecord, cancellationToken);

            string filePath = await _uploadFileService.SaveToUploadsFolder(file, cancellationToken);

            var patient = await _unitOfWork.Patients.GetByIdAsync(record.PatientId, cancellationToken) ?? throw new NotFoundException(nameof(Patient), record.PatientId);

            var newFile = patient.AddMedicalFile(record.Id, file.FileName, filePath, file.ContentType, fileType, fileDescription);
            
            await _unitOfWork.TrackAsAddedAsync(newFile, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return MedicalRecordService.ToFileDto(newFile);

        }



        public async Task<MedicalFileDownloadDto> DownloadFileAsync(Guid medicalRecordId, Guid fileId, CancellationToken cancellationToken = default)
        {
            var record = await _unitOfWork.MedicalRecords.GetWithDetailsAsync(medicalRecordId, cancellationToken)
                ?? throw new NotFoundException(nameof(MedicalRecord), medicalRecordId);

            var currentUserId = RequireCurrentUserId();

            await _accessValidationService.EnsureAccessAsync(record.PatientId, currentUserId, AccessScope.FullMedicalRecord, cancellationToken);

            var file = record.MedicalFiles.FirstOrDefault(f => f.Id == fileId) ?? throw new NotFoundException(nameof(MedicalFile), fileId);

            var stream = await _uploadFileService.OpenReadAsync(file.FilePath, cancellationToken);

            return new MedicalFileDownloadDto
            {
                Content = stream,
                ContentType = file.ContentType,
                FileName = file.FileName
            };

        }

        public async Task DeleteFileAsync(Guid medicalRecordId, Guid fileId, CancellationToken cancellationToken = default)
        {
            var record = await _unitOfWork.MedicalRecords.GetWithDetailsAsync(medicalRecordId, cancellationToken)
                ?? throw new NotFoundException(nameof(MedicalRecord), medicalRecordId);

            if (record.Status == MedicalRecordStatus.Superseded)
                throw new BusinessRuleViolationException("Can't delete files from a superseded (old) version of a medical record.");

            var currentUserId = RequireCurrentUserId();

            await _accessValidationService.EnsureAccessAsync(record.PatientId, currentUserId, AccessScope.FullMedicalRecord, cancellationToken);

            var file = record.MedicalFiles.FirstOrDefault(f => f.Id == fileId && !f.IsDeleted)
                ?? throw new NotFoundException(nameof(MedicalFile), fileId);

            file.SoftDelete();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

        }

    }
}
