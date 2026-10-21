using System.Globalization;
using Microsoft.EntityFrameworkCore;
using MediCore.Domain.Entities;

namespace MediCore.Infrastructure.Persistence.Seed;

public static class MedicationsSeeder
{
    public static async Task SeedAsync(AppDbContext context, string csvPath, CancellationToken cancellationToken = default)
    {
        if (await context.Medications.AnyAsync(cancellationToken))
            return;

        if (!File.Exists(csvPath))
            throw new FileNotFoundException("Medications CSV file was not found.", csvPath);

        var medications = new List<Medication>();

        using var reader = new StreamReader(csvPath);

        await reader.ReadLineAsync(cancellationToken);

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(line))
                continue;

            var columns = line.Split(',');

            if (columns.Length < 7)
                continue;

            var commercialNameEn = columns[0].Trim();
            var commercialNameAr = string.IsNullOrWhiteSpace(columns[1]) ? null : columns[1].Trim();
            var scientificName = string.IsNullOrWhiteSpace(columns[2]) ? null : columns[2].Trim();
            var manufacturer = string.IsNullOrWhiteSpace(columns[3]) ? null : columns[3].Trim();
            var drugClass = string.IsNullOrWhiteSpace(columns[4]) ? null : columns[4].Trim();
            var route = string.IsNullOrWhiteSpace(columns[5]) ? null : columns[5].Trim();

            if (!decimal.TryParse(columns[6].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var priceEgp))
                continue;

            medications.Add(new Medication(
                commercialNameEn,
                commercialNameAr,
                scientificName,
                manufacturer,
                drugClass,
                route,
                priceEgp));
        }

        if (medications.Count == 0)
            return;

        await context.Medications.AddRangeAsync(medications, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}