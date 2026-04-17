using System.Reflection;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NovaFashion.API.Features.Products;
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

            //Config Scalar UI with JWT Bearer Token
            //services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme);
            //services.AddAuthorizationBuilder();
            //services.AddOpenApi("v1", options => { options.AddDocumentTransformer<BearerSecuritySchemeTransformer>(); });

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

            ////Background Service
            //services.AddDbContextFactory<AppDbContext>((serviceProvider, options) =>
            //{
            //    options.UseSqlServer(connectionString);
            //    options.AddInterceptors(serviceProvider.GetRequiredService<AuditInterceptor>());
            //}, ServiceLifetime.Scoped);

            services.AddScoped<IProductRepository, ProductRepository>();

            return services;
        }
    }
}
