using NovaFashion.API.Configuration;

namespace NovaFashion.API.Infrastructure.Seed
{
    public static class SeedDataExtension
    {
        public static async Task SeedDatabaseAsync(this WebApplication app, AdminSettings settings)
        {
            using var scope = app.Services.CreateScope();
            await SeedData.InitializeAsync(scope.ServiceProvider, settings);
        }
    }
}
