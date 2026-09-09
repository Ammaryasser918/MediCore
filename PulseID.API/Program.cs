
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
            builder.Services.AddSwaggerGen(c =>
            {
                c.OperationFilter<RequesterHeaderOperationFilter>();
            });

            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(builder.Configuration);

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
            builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();




            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "PulseID API v1");
                c.RoutePrefix = string.Empty;
            });

            //app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseGlobalExceptionHandling();
            app.UseHttpsRedirection();
            app.UseAuthorization();


            app.MapControllers();
            
            app.Run();
        }
    }
}
