using Microsoft.EntityFrameworkCore;
using PulseID.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Infrastructure.Persistence.Seed
{
    public static class ChronicDiseasesSeeder
    {
        static readonly (string Name, string Description)[] Items =
        {
        // ---- Metabolic / endocrine ----
        ("Type 1 Diabetes Mellitus", "Autoimmune destruction of insulin-producing cells; requires lifelong insulin therapy."),
        ("Type 2 Diabetes Mellitus", "Chronic condition of insulin resistance and high blood glucose."),
        ("Hypothyroidism", "Underactive thyroid gland, commonly treated with levothyroxine."),
        ("Hyperthyroidism", "Overactive thyroid gland (e.g. Graves' disease)."),
        ("Obesity", "Chronic condition of excess body fat associated with multiple health risks."),
        ("Dyslipidemia (High Cholesterol)", "Abnormal blood lipid levels that raise cardiovascular risk."),
        ("Gout", "Inflammatory arthritis caused by uric acid crystal deposition in joints."),
        ("Polycystic Ovary Syndrome (PCOS)", "Hormonal disorder causing irregular periods, androgen excess and ovarian cysts."),
        ("Osteoporosis", "Reduced bone density leading to increased fracture risk."),

        // ---- Cardiovascular ----
        ("Hypertension", "Persistently elevated blood pressure."),
        ("Coronary Artery Disease", "Narrowing of the coronary arteries that supply the heart muscle."),
        ("Heart Failure", "Chronic inability of the heart to pump blood efficiently."),
        ("Atrial Fibrillation", "Irregular and often rapid heart rhythm."),
        ("Rheumatic Heart Disease", "Heart valve damage caused by rheumatic fever."),
        ("Peripheral Artery Disease", "Narrowed arteries reducing blood flow to the limbs."),
        ("Chronic Venous Insufficiency", "Impaired return of blood from the leg veins to the heart."),

        // ---- Respiratory ----
        ("Asthma", "Chronic inflammatory airway disease causing wheezing and shortness of breath."),
        ("Chronic Obstructive Pulmonary Disease (COPD)", "Progressive lung disease with persistent airflow limitation."),
        ("Chronic Bronchitis", "Long-term inflammation of the bronchi with productive cough."),
        ("Sleep Apnea", "Repeated breathing interruptions during sleep."),
        ("Pulmonary Fibrosis", "Progressive scarring of lung tissue."),
        ("Cystic Fibrosis", "Inherited disorder causing thick mucus in lungs and digestive tract."),

        // ---- Kidney / liver / GI ----
        ("Chronic Kidney Disease", "Gradual loss of kidney function over time."),
        ("Hepatitis B (Chronic)", "Long-term infection of the liver with hepatitis B virus."),
        ("Hepatitis C (Chronic)", "Long-term infection of the liver with hepatitis C virus."),
        ("Liver Cirrhosis", "Advanced scarring of the liver from chronic liver disease."),
        ("Non-Alcoholic Fatty Liver Disease", "Excess fat accumulation in the liver unrelated to alcohol."),
        ("Crohn's Disease", "Chronic inflammatory bowel disease that can affect any part of the GI tract."),
        ("Ulcerative Colitis", "Chronic inflammation of the colon and rectum."),
        ("Irritable Bowel Syndrome (IBS)", "Chronic functional bowel disorder with abdominal pain and altered bowel habits."),
        ("Gastroesophageal Reflux Disease (GERD)", "Chronic acid reflux into the esophagus."),
        ("Celiac Disease", "Autoimmune reaction to gluten that damages the small intestine."),

        // ---- Neurological / psychiatric ----
        ("Epilepsy", "Neurological disorder characterized by recurrent seizures."),
        ("Migraine (Chronic)", "Recurrent severe headaches, often with nausea and light sensitivity."),
        ("Parkinson's Disease", "Progressive neurodegenerative disorder affecting movement."),
        ("Multiple Sclerosis", "Autoimmune disease affecting the central nervous system."),
        ("Alzheimer's Disease / Dementia", "Progressive decline in memory and cognitive function."),
        ("Depression", "Chronic or recurrent mood disorder."),
        ("Anxiety Disorder", "Chronic excessive anxiety or worry."),
        ("Bipolar Disorder", "Mood disorder with episodes of mania and depression."),
        ("Schizophrenia", "Chronic psychiatric disorder affecting thought, perception and behavior."),

        // ---- Musculoskeletal / autoimmune ----
        ("Rheumatoid Arthritis", "Autoimmune inflammatory arthritis affecting the joints."),
        ("Osteoarthritis", "Degenerative joint disease caused by cartilage wear."),
        ("Systemic Lupus Erythematosus", "Autoimmune disease that can affect skin, joints, kidneys and other organs."),
        ("Ankylosing Spondylitis", "Chronic inflammatory arthritis mainly affecting the spine."),
        ("Fibromyalgia", "Chronic widespread musculoskeletal pain and fatigue."),
        ("Psoriasis", "Chronic autoimmune skin condition with scaly plaques."),
        ("Chronic Eczema (Atopic Dermatitis)", "Long-term inflammatory skin condition causing itchy, dry skin."),

        // ---- Blood / immune / infections / others ----
        ("Sickle Cell Disease", "Inherited blood disorder causing abnormally shaped red blood cells."),
        ("Thalassemia", "Inherited blood disorder with reduced hemoglobin production."),
        ("Hemophilia", "Inherited bleeding disorder due to clotting factor deficiency."),
        ("Chronic Anemia", "Persistently low hemoglobin due to iron deficiency or chronic disease."),
        ("HIV/AIDS", "Chronic viral infection that weakens the immune system."),
        ("Glaucoma", "Chronic eye disease with progressive optic nerve damage."),
        ("Chronic Sinusitis", "Persistent inflammation of the sinuses lasting over 12 weeks."),
        ("Benign Prostatic Hyperplasia", "Non-cancerous enlargement of the prostate gland."),
        ("Cancer (In Remission / Under Treatment)", "History of or ongoing cancer requiring long-term follow-up."),
    };

        public static async Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
        {
            var existing = await context.ChronicDiseases
                .Select(c => c.Name)
                .ToListAsync(cancellationToken);

            var existingNames = new HashSet<string>(existing, StringComparer.OrdinalIgnoreCase);

            var toAdd = Items
                .Where(i => !existingNames.Contains(i.Name))
                .Select(i => new ChronicDisease(i.Name, i.Description))
                .ToList();

            if (toAdd.Count == 0)
                return;

            await context.ChronicDiseases.AddRangeAsync(toAdd, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
    }

}
