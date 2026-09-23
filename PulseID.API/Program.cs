
using PulseID.API.Common;
using PulseID.API.Middleware;
using PulseID.Application;
using PulseID.Application.Common.Interfaces;
using PulseID.Domain.Entities;
using PulseID.Domain.Enums;
using PulseID.Infrastructure;
using PulseID.Infrastructure.Persistence;
using PulseID.Infrastructure.Persistence.Seed;

namespace PulseID.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.OperationFilter<RequesterHeaderOperationFilter>();
            });

            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(builder.Configuration);

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();




            var app = builder.Build();


            // Seed database.
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var csvPath = Path.Combine(AppContext.BaseDirectory, "medications.csv");

                await MedicationsSeeder.SeedAsync(context, csvPath);
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseGlobalExceptionHandling();
            app.UseHttpsRedirection();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
