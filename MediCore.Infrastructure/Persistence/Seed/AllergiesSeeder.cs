using Microsoft.EntityFrameworkCore;
using MediCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Infrastructure.Persistence.Seed
{
    public static class AllergiesSeeder
    {
        static readonly (string Name, string Description)[] Items =
        {
        // ---- Drug allergies ----
        ("Penicillin", "Allergy to penicillin-class antibiotics (e.g. amoxicillin, ampicillin). Can range from rash to anaphylaxis."),
        ("Cephalosporins", "Allergy to cephalosporin antibiotics (e.g. cefalexin, ceftriaxone). Cross-reactivity with penicillin is possible."),
        ("Sulfonamides (Sulfa Drugs)", "Allergy to sulfonamide antibiotics such as sulfamethoxazole-trimethoprim."),
        ("Aspirin", "Hypersensitivity to aspirin, may cause hives, swelling or bronchospasm."),
        ("NSAIDs (Ibuprofen, Diclofenac)", "Hypersensitivity to non-steroidal anti-inflammatory drugs."),
        ("Codeine / Opioids", "Allergic or pseudo-allergic reaction to codeine, morphine or related opioids."),
        ("Local Anesthetics", "Allergic reaction to local anesthetics such as lidocaine or procaine."),
        ("Contrast Dye (Iodine)", "Reaction to iodinated contrast media used in imaging."),
        ("Anticonvulsants (Carbamazepine, Phenytoin)", "Hypersensitivity reactions to anticonvulsant drugs, may include severe skin reactions."),
        ("Insulin", "Rare local or systemic allergic reaction to insulin preparations."),

        // ---- Food allergies ----
        ("Peanuts", "Food allergy to peanuts. A common cause of severe anaphylaxis."),
        ("Tree Nuts", "Allergy to almonds, walnuts, cashews, pistachios, hazelnuts and similar nuts."),
        ("Cow's Milk", "Allergy to cow's milk proteins. Different from lactose intolerance."),
        ("Eggs", "Allergy to egg proteins, more common in children."),
        ("Wheat", "Allergy to wheat proteins."),
        ("Gluten (Celiac Reaction)", "Immune reaction to gluten in wheat, barley and rye."),
        ("Soy", "Allergy to soybeans and soy-derived products."),
        ("Fish", "Allergy to finned fish such as salmon, tuna or cod."),
        ("Shellfish", "Allergy to crustaceans and mollusks such as shrimp, crab and lobster."),
        ("Sesame", "Allergy to sesame seeds and sesame oil/tahini."),
        ("Strawberries", "Food sensitivity that may cause hives or itching."),
        ("Chocolate", "Food sensitivity that may cause rash or headache."),

        // ---- Environmental / airborne ----
        ("Pollen", "Seasonal allergic rhinitis (hay fever) triggered by tree, grass or weed pollen."),
        ("Dust Mites", "Allergy to house dust mite droppings. Common cause of year-round rhinitis and asthma."),
        ("Mold", "Allergy to mold spores, indoor or outdoor."),
        ("Pet Dander (Cats / Dogs)", "Allergy to proteins found in animal skin flakes, saliva and urine."),
        ("Cockroach Allergen", "Allergy to cockroach droppings and body parts, a trigger of asthma."),
        ("Feathers", "Allergy to bird feathers and down bedding."),

        // ---- Insect / contact / other ----
        ("Bee / Wasp Venom", "Allergy to bee, wasp or hornet stings. May cause anaphylaxis."),
        ("Ant Bites (Fire Ants)", "Allergic reaction to ant venom."),
        ("Latex", "Allergy to natural rubber latex found in gloves, balloons and medical devices."),
        ("Nickel", "Contact dermatitis from nickel in jewelry, buckles and coins."),
        ("Fragrances / Perfumes", "Contact allergy or sensitivity to fragrance ingredients."),
        ("Hair Dye (PPD)", "Contact allergy to para-phenylenediamine found in hair dyes."),
        ("Adhesives / Medical Tape", "Contact dermatitis from adhesives used in bandages and tapes."),
        ("Chlorhexidine", "Allergy to chlorhexidine antiseptic."),
        ("Cold Urticaria", "Hives triggered by exposure to cold temperatures."),
        ("Sun (Photosensitivity)", "Skin allergic reaction triggered by sunlight exposure."),
    };

        public static async Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
        {
            var existing = await context.Allergies
                .Select(a => a.Name)
                .ToListAsync(cancellationToken);

            var existingNames = new HashSet<string>(existing, StringComparer.OrdinalIgnoreCase);

            var toAdd = Items
                .Where(i => !existingNames.Contains(i.Name))
                .Select(i => new Allergy(i.Name, i.Description))
                .ToList();

            if (toAdd.Count == 0)
                return;

            await context.Allergies.AddRangeAsync(toAdd, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
