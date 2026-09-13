using Microsoft.OpenApi.Models;
using PulseID.API.Common;
using PulseID.API.Middleware;
using PulseID.Application;
using PulseID.Application.Common.Interfaces;
using PulseID.Infrastructure;

namespace PulseID.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();


            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(builder.Configuration);


            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "PulseID API", Version = "v1" });
            });


            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
            builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
            builder.Services.AddMemoryCache();


            builder.Services.AddCors(options =>
            {
                options.AddPolicy("PulseIDApiCorsPolicy", policy =>
                {
                    policy.WithOrigins(
                        "https://localhost:7217",
                        "http://localhost:5215",
                        "https://localhost:7175",
                        "https://localhost:5173",
                        "http://localhost:5173",
                        "https://pulse-id-red.vercel.app"
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });

            var app = builder.Build();


            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "PulseID API v1");
                if (!app.Environment.IsDevelopment())
                {
                    c.RoutePrefix = string.Empty;
                }
            });

            app.UseGlobalExceptionHandling();
            app.UseHttpsRedirection();

            app.UseCors("PulseIDApiCorsPolicy");

            app.MapControllers();

            app.Run();
        }
    }
}