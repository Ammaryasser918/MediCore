using System;

namespace PulseID.Application.DTOs.Medications;

public class UpdateMedicationDto
{
    public string CommercialNameEn { get; set; } = default!;
    public string? CommercialNameAr { get; set; }
    public string? ScientificName { get; set; }
    public string? Manufacturer { get; set; }
    public string? DrugClass { get; set; }
    public string? Route { get; set; }
    public decimal PriceEgp { get; set; }
}