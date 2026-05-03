using System.Reflection;
using FastEndpoints.Security;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NovaFashion.API.Configuration;
using NovaFashion.API.Entities;
using NovaFashion.API.Features.Authentications;
using NovaFashion.API.Infrastructure.Persistence;
using NovaFashion.API.Infrastructure.Persistence.Interceptors;
using NovaFashion.API.Shared.Services;

namespace NovaFashion.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            //Add Controllers & API Behavior
            services.AddControllers();

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register all FluentValidation validators from the current executing assembly
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), lifetime: ServiceLifetime.Transient);
            
            // Register Cloudinary service
            services.AddScoped<ICloudinaryService, CloudinaryService>();
            return services;
        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<AuditInterceptor>();

            services.AddDbContext<AppDbContext>((serviceProvider, options) =>
            {
                options.UseSqlServer(connectionString);
                options.AddInterceptors(serviceProvider.GetRequiredService<AuditInterceptor>());
            });

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 1;
            });

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, JwtSettings jwtSettings)
        {
            services.AddSingleton(jwtSettings);
            services.AddAuthenticationJwtBearer(
            s =>  //JwtSigningOptions             
            {
                s.SigningKey = jwtSettings.SecretKey;   
            }, 
            
            b => //JwtBearerOptions
            {
                b.TokenValidationParameters.ValidateIssuer = true;
                b.TokenValidationParameters.ValidateAudience = true;
                b.TokenValidationParameters.ValidateLifetime = true;
                b.TokenValidationParameters.ValidIssuer = jwtSettings.Issuer;
                b.TokenValidationParameters.ValidAudience = jwtSettings.Audience;
                b.TokenValidationParameters.ClockSkew = TimeSpan.Zero;
            });
  
            services.AddAuthorization();

            //For every login request, jwt token using here
            services.Configure<JwtCreationOptions>(o =>
            {
                o.SigningKey = jwtSettings.SecretKey;
                o.Issuer = jwtSettings.Issuer;
                o.Audience = jwtSettings.Audience;
                //o.ExpireAt = DateTime.UtcNow.AddMinutes(jwtSettings.TokenExpiryInMinutes);
            });

            // Override jwtbeare scheme instead default cookie scheme
            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            });

            services.AddScoped<MyTokenService>();

            return services;
        }
    }
}
