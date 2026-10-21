using System;

namespace MediCore.Application.DTOs.Medications;

public class MedicationDto
{
    public Guid Id { get; set; }
    public string CommercialNameEn { get; set; } = default!;
    public string? CommercialNameAr { get; set; }
    public string? ScientificName { get; set; }
    public string? Manufacturer { get; set; }
    public string? DrugClass { get; set; }
    public string? Route { get; set; }
    public decimal PriceEgp { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}