using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MediCore.Application.Common.Interfaces;
using MediCore.Application.Interfaces;
using MediCore.Application.Services;
using MediCore.Infrastructure.ExternalServices.JWT;
using MediCore.Infrastructure.ExternalServices.Notifications.ViaGmail;
using MediCore.Infrastructure.Identity;
using MediCore.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {

            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection String 'Default Connection' Not Found.");

            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

            services.AddScoped<IUnitOfWork, UnitOfWork>();


            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

            services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            services.AddScoped<IIdentityService, IdentityService>();

            services.Configure<EmailOptions>(configuration.GetSection("Email"));

            services.AddScoped<INotificationChannelStrategy, EmailNotificationStrategy>();

            services.AddScoped<INotificationSender, NotificationSender>();


            return services;

        }
    }
}
