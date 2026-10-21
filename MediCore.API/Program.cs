using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MediCore.API.Authorization;
using MediCore.API.Common;
using MediCore.API.Middleware;
using MediCore.Application;
using MediCore.Application.Common.Interfaces;
using MediCore.Infrastructure;
using MediCore.Infrastructure.Identity;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.RateLimiting;
using MediCore.Infrastructure.Persistence;
using MediCore.Infrastructure.Persistence.Seed;
using Microsoft.Extensions.FileProviders;

namespace MediCore.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            
            builder.Services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                string GetUserPartitionKey(HttpContext httpContext)
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
                    var partitionKey = GetUserPartitionKey(httpContext);
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
                    var partitionKey = GetUserPartitionKey(httpContext);
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
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "MediCore API", Version = "v1" });

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
            builder.Services.AddScoped<IFileStorageService, HostFileStorageService>();

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("PatientOwnerOrAdmin", policy => policy.Requirements.Add(new OwnerShipRequirement()));
                options.AddPolicy("DoctorOwnerOrAdmin", policy => policy.Requirements.Add(new OwnerShipRequirement()));
                options.AddPolicy("PharmacistOwnerOrAdmin", policy => policy.Requirements.Add(new OwnerShipRequirement()));
                options.AddPolicy("MedicalRecordOwnerOrAdmin", policy => policy.Requirements.Add(new OwnerShipRequirement()));
                options.AddPolicy("LabTestRequestOwnerOrAdmin", policy => policy.Requirements.Add(new OwnerShipRequirement()));
                options.AddPolicy("LabTechnicianOwnerOrAdmin", policy => policy.Requirements.Add(new OwnerShipRequirement()));
                options.AddPolicy("AccessRequestOwnerOrAdmin", policy => policy.Requirements.Add(new OwnerShipRequirement()));
                options.AddPolicy("PrescriptionOwnerOrAdmin", policy => policy.Requirements.Add(new OwnerShipRequirement()));
            });
            //builder.Services.AddMemoryCache();


            builder.Services.AddScoped<IAuthorizationHandler, PatientOwnerOrAdminHandler>();
            builder.Services.AddScoped<IAuthorizationHandler, DoctorOwnerOrAdminHandler>();
            builder.Services.AddScoped<IAuthorizationHandler, PharmacistOwnerOrAdminHandler>();
            builder.Services.AddScoped<IAuthorizationHandler, MedicalRecordOwnerOrAdminHandler>();
            builder.Services.AddScoped<IAuthorizationHandler, LabTestRequestOwnerOrAdminHandler>();
            builder.Services.AddScoped<IAuthorizationHandler, LabTechnicianOwnerOrAdminHandler>();
            builder.Services.AddScoped<IAuthorizationHandler, AccessRequestOwnerOrAdminHandler>();
            builder.Services.AddScoped<IAuthorizationHandler, PrescriptionOwnerOrAdminHandler>();


            builder.Services.AddCors(options =>
            {
                options.AddPolicy("MediCoreApiCorsPolicy", policy =>
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

            app.UseStaticFiles();

            app.UseRouting();

            app.UseGlobalExceptionHandling();


            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    // Seed Super Admin
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

                    await IdentityDataSeeder.SeedSuperAdminAsync(userManager, roleManager);
                    
                    // Seed database.
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var csvPath = Path.Combine(AppContext.BaseDirectory, "medications.csv");

                    await MedicationsSeeder.SeedAsync(context, csvPath);

                    await AllergiesSeeder.SeedAsync(context);
                    await ChronicDiseasesSeeder.SeedAsync(context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }



            

            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "MediCore API v1");
                if (!app.Environment.IsDevelopment())
                {
                    c.RoutePrefix = string.Empty;
                }
            });
            

            app.UseStatusCodePages(async statusCodeContext =>
            {
                var response = statusCodeContext.HttpContext.Response;
                if (!response.HasStarted && string.IsNullOrEmpty(response.ContentType))
                {
                    var statusCode = response.StatusCode;
                    var error = ((HttpStatusCode)statusCode).ToString();
                    var message = statusCode switch
                    {
                        401 => "Authentication token is missing or invalid.",
                        403 => "You do not have permission to access this resource.",
                        404 => "The requested resource was not found.",
                        405 => "The HTTP method is not allowed for this endpoint.",
                        _ => $"An error occurred (HTTP {statusCode})."
                    };

                    response.ContentType = "application/json";
                    var payload = JsonSerializer.Serialize(new
                    {
                        status = statusCode,
                        error,
                        message
                    });
                    await response.WriteAsync(payload);
                }
            });

            app.UseHttpsRedirection();

            var uploadContentTypes = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
            uploadContentTypes.Mappings[".dcm"] = "application/dicom";

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(StoragePaths.GetRoot(app.Configuration, app.Environment)),
                RequestPath = StoragePaths.RequestPath,
                ContentTypeProvider = uploadContentTypes,
                OnPrepareResponse = ctx => ctx.Context.Response.Headers["X-Content-Type-Options"] = "nosniff"
            });

            app.UseCors("MediCoreApiCorsPolicy");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRateLimiter();


            app.MapControllers();

            app.Run();
        }
    }
}