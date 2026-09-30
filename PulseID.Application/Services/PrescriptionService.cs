using PulseID.Application.Common.Exceptions;
using PulseID.Application.DTOs.Prescriptions;
using PulseID.Application.Interfaces;
using PulseID.Application.Interfaces.Services;
using PulseID.Domain.Entities;
using PulseID.Domain.Enums;

namespace PulseID.Application.Services;

public class PrescriptionService : IPrescriptionService
{
    readonly IUnitOfWork _unitOfWork;

    public PrescriptionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<PrescriptionDto>> GetByMedicalRecordAsync(Guid medicalRecordId, CancellationToken cancellationToken = default)
    {
        var prescriptions = await _unitOfWork.Prescriptions.GetByMedicalRecordAsync(medicalRecordId, cancellationToken);

        return prescriptions.Select(ToDto).ToList();
    }

    public async Task<IReadOnlyList<PrescriptionDto>> GetByStatusAsync(PrescriptionStatus status, CancellationToken cancellationToken = default)
    {
        var prescriptions = await _unitOfWork.Prescriptions.GetByStatusAsync(status, cancellationToken);

        return prescriptions.Select(ToDto).ToList();
    }

    public async Task<PrescriptionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var prescription = await _unitOfWork.Prescriptions.GetByIdAsync(id, cancellationToken);

        return prescription is null ? null : ToDto(prescription);
    }

    public async Task<PrescriptionDto?> GetWithItemsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var prescription = await _unitOfWork.Prescriptions.GetWithItemsAsync(id, cancellationToken);

        return prescription is null ? null : ToDto(prescription);
    }

    public async Task<PrescriptionDto> CreateAsync(CreatePrescriptionDto dto, CancellationToken cancellationToken = default)
    {
        var prescription = new Prescription(dto.MedicalRecordId, dto.Notes);

        await _unitOfWork.Prescriptions.AddAsync(prescription, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToDto(prescription);
    }

    public async Task<PrescriptionDto> UpdateAsync(Guid id, UpdatePrescriptionDto dto, CancellationToken cancellationToken = default)
    {
        var prescription = await _unitOfWork.Prescriptions.GetByIdAsync(id, cancellationToken);

        if (prescription is null)
            throw new NotFoundException($"Prescription with id '{id}' was not found.");

        prescription.UpdateDetails(dto.Notes);

        _unitOfWork.Prescriptions.Update(prescription);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToDto(prescription);
    }

    public async Task AddItemAsync(Guid prescriptionId, AddPrescriptionItemDto dto, CancellationToken cancellationToken = default)
    {
        var prescription = await _unitOfWork.Prescriptions.GetWithItemsAsync(prescriptionId, cancellationToken);

        if (prescription is null)
            throw new NotFoundException($"Prescription with id '{prescriptionId}' was not found.");

        prescription.AddItem(dto.MedicationId, dto.Dosage, dto.Duration, dto.Instructions);

        _unitOfWork.Prescriptions.Update(prescription);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateItemAsync(Guid prescriptionId, Guid itemId, UpdatePrescriptionItemDto dto, CancellationToken cancellationToken = default)
    {
        var prescription = await _unitOfWork.Prescriptions.GetWithItemsAsync(prescriptionId, cancellationToken);

        if (prescription is null)
            throw new NotFoundException($"Prescription with id '{prescriptionId}' was not found.");

        var item = prescription.PrescriptionItems.FirstOrDefault(i => i.Id == itemId);

        if (item is null)
            throw new NotFoundException($"Prescription item with id '{itemId}' was not found.");

        prescription.UpdateItem(itemId, dto.MedicationId, dto.Dosage, dto.Duration, dto.Instructions);

        _unitOfWork.Prescriptions.Update(prescription);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveItemAsync(Guid prescriptionId, Guid itemId, CancellationToken cancellationToken = default)
    {
        var prescription = await _unitOfWork.Prescriptions.GetWithItemsAsync(prescriptionId, cancellationToken);

        if (prescription is null)
            throw new NotFoundException($"Prescription with id '{prescriptionId}' was not found.");

        var item = prescription.PrescriptionItems.FirstOrDefault(i => i.Id == itemId);

        if (item is null)
            throw new NotFoundException($"Prescription item with id '{itemId}' was not found.");

        prescription.RemoveItem(itemId);

        _unitOfWork.Prescriptions.Update(prescription);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task VerifyAsync(Guid prescriptionId, CancellationToken cancellationToken = default)
    {
        var prescription = await _unitOfWork.Prescriptions.GetWithItemsAsync(prescriptionId, cancellationToken);

        if (prescription is null)
            throw new NotFoundException($"Prescription with id '{prescriptionId}' was not found.");

        prescription.MarkAsVerified();

        _unitOfWork.Prescriptions.Update(prescription);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DispenseItemAsync(Guid prescriptionId, Guid itemId, Guid pharmacistId, CancellationToken cancellationToken = default)
    {
        var prescription = await _unitOfWork.Prescriptions.GetWithItemsAsync(prescriptionId, cancellationToken);

        if (prescription is null)
            throw new NotFoundException($"Prescription with id '{prescriptionId}' was not found.");

        var item = prescription.PrescriptionItems.FirstOrDefault(i => i.Id == itemId);

        if (item is null)
            throw new NotFoundException($"Prescription item with id '{itemId}' was not found.");

        item.Dispense(pharmacistId);
        prescription.RefreshStatus();

        _unitOfWork.Prescriptions.Update(prescription);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task CancelAsync(Guid prescriptionId, CancellationToken cancellationToken = default)
    {
        var prescription = await _unitOfWork.Prescriptions.GetByIdAsync(prescriptionId, cancellationToken);

        if (prescription is null)
            throw new NotFoundException($"Prescription with id '{prescriptionId}' was not found.");

        prescription.Cancel();

        _unitOfWork.Prescriptions.Update(prescription);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid prescriptionId, CancellationToken cancellationToken = default)
    {
        var prescription = await _unitOfWork.Prescriptions.GetByIdAsync(prescriptionId, cancellationToken);

        if (prescription is null)
            throw new NotFoundException($"Prescription with id '{prescriptionId}' was not found.");

        _unitOfWork.Prescriptions.Delete(prescription);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task HardDeleteAsync(Guid prescriptionId, CancellationToken cancellationToken = default)
    {
        var prescription = await _unitOfWork.Prescriptions.GetByIdWithDeletedAsync(prescriptionId, cancellationToken)
            ?? throw new NotFoundException($"Prescription with id '{prescriptionId}' was not found.");
        _unitOfWork.Prescriptions.HardDelete(prescription);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PrescriptionDto>> GetDeletedAsync(CancellationToken cancellationToken = default)
    {
        var prescriptions = await _unitOfWork.Prescriptions.GetDeletedAsync(cancellationToken);
        return prescriptions.Select(ToDto).ToList();
    }

    public async Task RestorePrescriptionAsync(Guid prescriptionId, CancellationToken cancellationToken = default)
    {
        var prescription = await _unitOfWork.Prescriptions.GetDeletedByIdAsync(prescriptionId, cancellationToken)
            ?? throw new NotFoundException("Deleted prescription record not found.");
        _unitOfWork.Prescriptions.Restore(prescription);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }


    static PrescriptionDto ToDto(Prescription prescription)
    {
        return new PrescriptionDto
        {
            Id = prescription.Id,
            MedicalRecordId = prescription.MedicalRecordId,
            PatientId = prescription.MedicalRecord?.PatientId ?? Guid.Empty,
            DoctorId = prescription.MedicalRecord?.DoctorId ?? Guid.Empty,
            IssueDate = prescription.IssueDate,
            DispensedAt = prescription.DispensedAt,
            Status = prescription.Status,
            Notes = prescription.Notes,
            Items = prescription.PrescriptionItems.Select(ToItemDto).ToList(),
            CreatedAt = prescription.CreatedAt,
            UpdatedAt = prescription.UpdatedAt
        };
    }

    static PrescriptionItemDto ToItemDto(PrescriptionItem item)
    {
        return new PrescriptionItemDto
        {
            Id = item.Id,
            MedicationId = item.MedicationId,
            MedicationName = item.Medication.CommercialNameEn,
            Dosage = item.Dosage,
            Duration = item.Duration,
            Instructions = item.Instructions,
            Status = item.Status,
            DispensedByPharmacistId = item.DispensedByPharmacistId,
            DispensedAt = item.DispensedAt
        };
    }
}