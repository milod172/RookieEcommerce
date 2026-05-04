using Microsoft.AspNetCore.Identity;
using NovaFashion.API.Configuration;
using NovaFashion.API.Entities;
using NovaFashion.API.Entities.Enum;

namespace NovaFashion.API.Infrastructure.Seed
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services, AdminSettings settings)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger(nameof(SeedData));

            string[] roles = [Role.Admin.ToString(), Role.Customer.ToString()];
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(role));
                    if (result.Succeeded)
                        logger.LogInformation("Đã tạo role: {Role}", role);
                    else
                    {
                        var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                        logger.LogError("Tạo role {Role} thất bại: {Errors}", role, errors);
                    }
                }
            }

            var existing = await userManager.FindByEmailAsync(settings.Email);
            if (existing != null)
            {
                if (!await userManager.IsInRoleAsync(existing, Role.Admin.ToString()))
                {
                    await userManager.AddToRoleAsync(existing, Role.Admin.ToString());
                    logger.LogInformation("Đã gán role Admin cho user hiện tại: {Email}", settings.Email);
                }
                else
                {
                    logger.LogInformation("Tài khoản Admin đã tồn tại, bỏ qua seed.");
                }
                return;
            }

            var adminUser = new ApplicationUser
            {
                FirstName = settings.FirstName,
                LastName = settings.LastName,
                Email = settings.Email,
                UserName = settings.Email,
                EmailConfirmed = true,
                IsActive = true
            };

            var createResult = await userManager.CreateAsync(adminUser, settings.Password);
            if (createResult.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, Role.Admin.ToString());
                logger.LogInformation("Đã tạo tài khoản Admin: {Email}", settings.Email);
            }
            else
            {
                var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                logger.LogError("Tạo tài khoản Admin thất bại: {Errors}", errors);
            }
        }
    }
}
