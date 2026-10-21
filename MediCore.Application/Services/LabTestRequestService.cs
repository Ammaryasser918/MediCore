using MediCore.Application.Common.Exceptions;
using MediCore.Application.DTOs.Labs;
using MediCore.Application.Interfaces;
using MediCore.Application.Interfaces.Repositories;
using MediCore.Application.Interfaces.Services;
using MediCore.Domain.Entities;
using MediCore.Domain.Enums;

namespace MediCore.Application.Services;

public class LabTestRequestService : ILabTestRequestService
{
    readonly IUnitOfWork _unitOfWork;

    public LabTestRequestService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<LabTestRequestDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var requests = await _unitOfWork.LabTestRequests.GetAllWithDetailsAsync(cancellationToken);

        return requests.Select(ToDto).ToList();
    }

    public async Task<LabTestRequestDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var request = await _unitOfWork.LabTestRequests.GetWithDetailsAsync(id, cancellationToken);

        return request is null ? null : ToDto(request);
    }

    public async Task<IReadOnlyList<LabTestRequestDto>> GetByPatientAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        var requests = await _unitOfWork.LabTestRequests.GetByPatientAsync(patientId, cancellationToken);

        return requests.Select(ToDto).ToList();
    }

    public async Task<IReadOnlyList<LabTestRequestDto>> GetByDoctorAsync(Guid doctorId, CancellationToken cancellationToken = default)
    {
        var requests = await _unitOfWork.LabTestRequests.GetByDoctorAsync(doctorId, cancellationToken);

        return requests.Select(ToDto).ToList();
    }

    public async Task<IReadOnlyList<LabTestRequestDto>> GetByLabAsync(Guid labId, CancellationToken cancellationToken = default)
    {
        var requests = await _unitOfWork.LabTestRequests.GetByLabAsync(labId, cancellationToken);

        return requests.Select(ToDto).ToList();
    }

    public async Task<IReadOnlyList<LabTestRequestDto>> GetByStatusAsync(LabTestRequestStatus status, CancellationToken cancellationToken = default)
    {
        var requests = await _unitOfWork.LabTestRequests.GetByStatusAsync(status, cancellationToken);

        return requests.Select(ToDto).ToList();
    }

    public async Task<LabTestRequestDto> CreateAsync(CreateLabTestRequestDto dto, CancellationToken cancellationToken = default)
    {
        var request = new LabTestRequest(
            dto.PatientId,
            dto.DoctorId,
            dto.LabTestId,
            dto.LabId,
            dto.Notes);

        await _unitOfWork.LabTestRequests.AddAsync(request, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var createdRequest = await _unitOfWork.LabTestRequests.GetWithDetailsAsync(request.Id, cancellationToken);

        if (createdRequest is null)
            throw new NotFoundException($"Lab test request with id '{request.Id}' was not found.");

        return ToDto(createdRequest);
    }

    public async Task<LabTestRequestDto> UpdateAsync(Guid id, UpdateLabTestRequestDto dto, CancellationToken cancellationToken = default)
    {
        var request = await _unitOfWork.LabTestRequests.GetWithDetailsAsync(id, cancellationToken);

        if (request is null)
            throw new NotFoundException($"Lab test request with id '{id}' was not found.");

        request.Update(
            dto.PatientId,
            dto.DoctorId,
            dto.LabTestId,
            dto.LabId,
            dto.Notes);

        _unitOfWork.LabTestRequests.Update(request);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updatedRequest = await _unitOfWork.LabTestRequests.GetWithDetailsAsync(id, cancellationToken);

        if (updatedRequest is null)
            throw new NotFoundException($"Lab test request with id '{id}' was not found.");

        return ToDto(updatedRequest);
    }

    public async Task AssignLabAsync(Guid id, Guid labId, CancellationToken cancellationToken = default)
    {
        var request = await _unitOfWork.LabTestRequests.GetByIdAsync(id, cancellationToken);

        if (request is null)
            throw new NotFoundException($"Lab test request with id '{id}' was not found.");

        request.AssignLab(labId);

        _unitOfWork.LabTestRequests.Update(request);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task SendToLabAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var request = await _unitOfWork.LabTestRequests.GetByIdAsync(id, cancellationToken);

        if (request is null)
            throw new NotFoundException($"Lab test request with id '{id}' was not found.");

        request.SendToLab();

        _unitOfWork.LabTestRequests.Update(request);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task CollectSampleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var request = await _unitOfWork.LabTestRequests.GetByIdAsync(id, cancellationToken);

        if (request is null)
            throw new NotFoundException($"Lab test request with id '{id}' was not found.");

        request.CollectSample();

        _unitOfWork.LabTestRequests.Update(request);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task StartProcessingAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var request = await _unitOfWork.LabTestRequests.GetByIdAsync(id, cancellationToken);

        if (request is null)
            throw new NotFoundException($"Lab test request with id '{id}' was not found.");

        request.StartProcessing();

        _unitOfWork.LabTestRequests.Update(request);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UploadResultAsync(Guid id, string resultFileUrl, CancellationToken cancellationToken = default)
    {
        var request = await _unitOfWork.LabTestRequests.GetByIdAsync(id, cancellationToken);

        if (request is null)
            throw new NotFoundException($"Lab test request with id '{id}' was not found.");

        request.UploadResult(resultFileUrl);

        _unitOfWork.LabTestRequests.Update(request);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UploadPatientResultAsync(Guid id, string resultFileUrl, CancellationToken cancellationToken = default)
    {
        var request = await _unitOfWork.LabTestRequests.GetByIdAsync(id, cancellationToken);

        if (request is null)
            throw new NotFoundException($"Lab test request with id '{id}' was not found.");

        request.UploadPatientResult(resultFileUrl);

        _unitOfWork.LabTestRequests.Update(request);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RejectAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var request = await _unitOfWork.LabTestRequests.GetByIdAsync(id, cancellationToken);

        if (request is null)
            throw new NotFoundException($"Lab test request with id '{id}' was not found.");

        request.Reject();

        _unitOfWork.LabTestRequests.Update(request);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var request = await _unitOfWork.LabTestRequests.GetByIdAsync(id, cancellationToken);

        if (request is null)
            throw new NotFoundException($"Lab test request with id '{id}' was not found.");

        _unitOfWork.LabTestRequests.Delete(request);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    static LabTestRequestDto ToDto(LabTestRequest request)
    {
        return new LabTestRequestDto
        {
            Id = request.Id,
            PatientId = request.PatientId,
            PatientName = request.Patient.FullName,

            DoctorId = request.DoctorId,
            DoctorName = request.Doctor.FullName,

            LabTestId = request.LabTestId,
            LabTestName = request.LabTest.Name,

            LabId = request.LabId,
            LabName = request.Lab?.Name,

            Status = request.Status,
            Notes = request.Notes,

            ResultFileUrl = request.ResultFileUrl,

            RequestedAt = request.RequestedAt,
            SampleCollectedAt = request.SampleCollectedAt,
            CompletedAt = request.CompletedAt,

            CreatedAt = request.CreatedAt,
            UpdatedAt = request.UpdatedAt
        };
    }
}