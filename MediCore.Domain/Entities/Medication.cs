using MediCore.Domain.Common;

namespace MediCore.Domain.Entities;

public class Medication : BaseEntity
{
    public string CommercialNameEn { get; private set; } = default!;
    public string? CommercialNameAr { get; private set; }

    public string? ScientificName { get; private set; }

    public string? Manufacturer { get; private set; }

    public string? DrugClass { get; private set; }

    public string? Route { get; private set; }

    public decimal PriceEgp { get; private set; }

    readonly List<PrescriptionItem> _prescriptionItems = new();
    public IReadOnlyCollection<PrescriptionItem> PrescriptionItems => _prescriptionItems.AsReadOnly();

    Medication() { }

    public Medication(
        string commercialNameEn,
        string? commercialNameAr,
        string? scientificName,
        string? manufacturer,
        string? drugClass,
        string? route,
        decimal priceEgp)
    {
        if (string.IsNullOrWhiteSpace(commercialNameEn))
            throw new ArgumentException("Commercial name is required.", nameof(commercialNameEn));

        if (priceEgp < 0)
            throw new ArgumentException("Price cannot be negative.", nameof(priceEgp));

        CommercialNameEn = commercialNameEn;
        CommercialNameAr = commercialNameAr;
        ScientificName = scientificName;
        Manufacturer = manufacturer;
        DrugClass = drugClass;
        Route = route;
        PriceEgp = priceEgp;
    }

    public void UpdateDetails(string commercialNameEn, string? commercialNameAr, string? scientificName, string? manufacturer, string? drugClass, string? route, decimal priceEgp)
    {
        if (string.IsNullOrWhiteSpace(commercialNameEn))
            throw new ArgumentException("Commercial name is required.", nameof(commercialNameEn));

        if (priceEgp < 0)
            throw new ArgumentException("Price cannot be negative.", nameof(priceEgp));

        CommercialNameEn = commercialNameEn;
        CommercialNameAr = commercialNameAr;
        ScientificName = scientificName;
        Manufacturer = manufacturer;
        DrugClass = drugClass;
        Route = route;
        PriceEgp = priceEgp;

        MarkAsUpdated();
    }
}