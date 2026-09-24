using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PulseID.API.Authorization;
using PulseID.API.Common;
using PulseID.API.Middleware;
using PulseID.Application;
using PulseID.Application.Common.Interfaces;
using PulseID.Infrastructure;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using PulseID.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace PulseID.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            
            builder.Services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                string GetUserPartitiohnKey(HttpContext httpContext)
                {
                    return httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous";
                }

                // 5 attempts per IP per minute
                options.AddPolicy("AuthLimiter", httpContext =>
                {
                    var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: ip,
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0
                        }
                        );
                });

                // 20 requests per user per minute
                options.AddPolicy("BigDataLimiter", httpContext =>
                {
                    var partitionKey = GetUserPartitiohnKey(httpContext);
                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: partitionKey,
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 20,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0
                        });
                });

                // 60 requests per user per minute
                options.AddPolicy("SmallDataLimiter", httpContext =>
                {
                    var partitionKey = GetUserPartitiohnKey(httpContext);
                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: partitionKey,
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 60,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0
                        });
                });

                options.AddPolicy("OTPLimiter", httpContext =>
                {
                    var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: ip,
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 10,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0
                        }
                        );
                });

            });


            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddApplication();

            builder.Services.AddInfrastructure(builder.Configuration);


            var jwtSettings = builder.Configuration.GetSection("Jwt");
            var secretKey = jwtSettings["Secret"] ?? builder.Configuration["Jwt:Secret"];

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)),
                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = ClaimTypes.NameIdentifier
                };
            });


            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "PulseID API", Version = "v1" });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter: Bearer {your JWT token}"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });




            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
            builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("PatientOwnerOrAdmin", policy => policy.Requirements.Add(new OwnerShipRequirement()));
                options.AddPolicy("DoctorOwnerOrAdmin", policy => policy.Requirements.Add(new OwnerShipRequirement()));
                options.AddPolicy("PharmacistOwnerOrAdmin", policy => policy.Requirements.Add(new OwnerShipRequirement()));
                options.AddPolicy("MedicalRecordOwnerOrAdmin", policy => policy.Requirements.Add(new OwnerShipRequirement()));
            });
            //builder.Services.AddMemoryCache();


            builder.Services.AddScoped<IAuthorizationHandler, PatientOwnerOrAdminHandler>();
            builder.Services.AddScoped<IAuthorizationHandler, DoctorOwnerOrAdminHandler>();
            builder.Services.AddScoped<IAuthorizationHandler, PharmacistOwnerOrAdminHandler>();
            builder.Services.AddScoped<IAuthorizationHandler, MedicalRecordOwnerOrAdminHandler>();


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

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

                    await IdentityDataSeeder.SeedSuperAdminAsync(userManager, roleManager);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the SuperAdmin user.");
                }
            }


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
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}