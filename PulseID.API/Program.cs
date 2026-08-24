
using PulseID.API.Common;
using PulseID.API.Middleware;
using PulseID.Application;
using PulseID.Application.Common.Interfaces;
using PulseID.Domain.Entities;
using PulseID.Domain.Enums;
using PulseID.Infrastructure;
using PulseID.Infrastructure.Persistence;

namespace PulseID.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(builder.Configuration);

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();




            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            //app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseGlobalExceptionHandling();
            app.UseAuthorization();


            app.MapControllers();
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // «· √ﬂœ „‰ ≈‰ «·œ« «»Ì“ Ê„ÊœÌ·« Â« Ã«Â“…
                dbContext.Database.EnsureCreated();

                var doctorUserId = Guid.Parse("12345678-1234-1234-1234-1234567890ab");
                var patientUserId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890");
                var hospitalId = Guid.Parse("f9e8d7c6-b5a4-3210-fedc-ba0987654321");

                // 1. “—⁄ «·œﬂ Ê— »«·ﬂÊ‰” —ﬂ Ê— «·Œ«’ »ÌÂ
                if (!dbContext.Doctors.Any(d => d.UserId == doctorUserId))
                {
                    var doctor = new Doctor(
                        userId: doctorUserId,
                        fullName: "Dr. Ahmed Test",
                        syndicateNumber: "123456",
                        syndicateDocumentPath: "uploads/docs/test.pdf",
                        nationalId: "29912345678901",
                        specialization: "General",
                        phoneNumber: "01000000000",
                        departmentId: Guid.NewGuid()
                    );
                    dbContext.Doctors.Add(doctor);
                }

                // 2. “—⁄ «·„—Ì÷ »«·ﬂÊ‰” —ﬂ Ê— «·Œ«’ »ÌÂ
                if (!dbContext.Patients.Any(p => p.UserId == patientUserId))
                {
                    var patient = new Patient(
                        userId: patientUserId,
                        nationalId: "30012345678902",
                        fullName: "Patient Mohamed",
                        dateOfBirth: new DateTime(1995, 5, 15),
                        gender: Gender.Male, //  √ﬂœ „‰ «·‹ Enum «·Œ«’ »«·‰Ê⁄ ·Ê „Œ ·› ⁄‰œﬂ
                        phoneNumber: "01111111111"
                    );
                    dbContext.Patients.Add(patient);
                }

                // 3. “—⁄ «·„” ‘›Ï »«·ﬂÊ‰” —ﬂ Ê— «·Œ«’ »ÌÂ«
                // 3. “—⁄ «·„” ‘›Ï »«·ﬂÊ‰” —ﬂ Ê— «·Œ«’ »ÌÂ«
                // 3. “—⁄ «·„” ‘›Ï »«·ﬂÊ‰” —ﬂ Ê— «·Œ«’ »ÌÂ«
                var hospital = dbContext.Hospitals.FirstOrDefault(h => h.Name == "PulseID Central Hospital");
                if (hospital == null)
                {
                    hospital = new Hospital(
                        name: "PulseID Central Hospital",
                        address: "Cairo, Egypt",
                        phoneNumber: "0223344556",
                        licenseNumber: "LIC-12345"
                    );

                    dbContext.Hospitals.Add(hospital);
                    dbContext.SaveChanges(); // Õ›Ÿ ⁄‘«‰ «·œ« «»Ì“  Ê·œ «·‹ Id √Ê Ê„« Ìﬂ

                    // «ÿ»⁄ «·‹ Id ›Ì «·‹ Output ⁄‘«‰  «ŒœÂ  ÕÿÂ ›Ì «·‹ Swagger
                    System.Diagnostics.Debug.WriteLine($"[SEED INFO] Hospital ID: {hospital.Id}");
                    Console.WriteLine($"[SEED INFO] Hospital ID: {hospital.Id}");
                }

                dbContext.SaveChanges();
            }
            app.Run();
        }
    }
}
