using PulseID.Application.Common.Exceptions;
using PulseID.Application.DTOs.Medications;
using PulseID.Application.Interfaces;
using PulseID.Application.Interfaces.Services;
using PulseID.Domain.Entities;

namespace PulseID.Application.Services;

public class MedicationService : IMedicationService
{
    readonly IUnitOfWork _unitOfWork;

    public MedicationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<MedicationDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var medications = await _unitOfWork.Medications.SearchAsync(searchTerm, cancellationToken);
        return medications.Select(ToDto).ToList();
    }

    public async Task<MedicationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var medication = await _unitOfWork.Medications.GetByIdAsync(id, cancellationToken);
        return medication is null ? null : ToDto(medication);
    }

    public async Task<MedicationDto> CreateAsync(CreateMedicationDto dto, CancellationToken cancellationToken = default)
    {
        var medication = new Medication(
            dto.CommercialNameEn,
            dto.CommercialNameAr,
            dto.ScientificName,
            dto.Manufacturer,
            dto.DrugClass,
            dto.Route,
            dto.PriceEgp);

        await _unitOfWork.Medications.AddAsync(medication, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToDto(medication);
    }

    public async Task<MedicationDto> UpdateAsync(Guid id, UpdateMedicationDto dto, CancellationToken cancellationToken = default)
    {
        var medication = await _unitOfWork.Medications.GetByIdAsync(id, cancellationToken);

        if (medication is null)
            throw new NotFoundException($"Medication with id '{id}' was not found.");

        medication.UpdateDetails(
            dto.CommercialNameEn,
            dto.CommercialNameAr,
            dto.ScientificName,
            dto.Manufacturer,
            dto.DrugClass,
            dto.Route,
            dto.PriceEgp);

        _unitOfWork.Medications.Update(medication);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToDto(medication);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var medication = await _unitOfWork.Medications.GetByIdAsync(id, cancellationToken);

        if (medication is null)
            throw new NotFoundException($"Medication with id '{id}' was not found.");

        _unitOfWork.Medications.Delete(medication);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    static MedicationDto ToDto(Medication medication)
    {
        return new MedicationDto
        {
            Id = medication.Id,
            CommercialNameEn = medication.CommercialNameEn,
            CommercialNameAr = medication.CommercialNameAr,
            ScientificName = medication.ScientificName,
            Manufacturer = medication.Manufacturer,
            DrugClass = medication.DrugClass,
            Route = medication.Route,
            PriceEgp = medication.PriceEgp,
            CreatedAt = medication.CreatedAt,
            UpdatedAt = medication.UpdatedAt
        };
    }
}