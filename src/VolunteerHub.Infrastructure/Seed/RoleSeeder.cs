using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VolunteerHub.Contracts.Constants;
using VolunteerHub.Infrastructure.Identity;

namespace VolunteerHub.Infrastructure.Seed;

public static class RoleSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(typeof(RoleSeeder));

        var roles = new[]
        {
            AppRoles.Admin,
            AppRoles.Organizer,
            AppRoles.Volunteer,
            AppRoles.Sponsor
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var result = await roleManager.CreateAsync(new ApplicationRole(role));
                if (result.Succeeded)
                {
                    logger.LogInformation("Created role {Role}", role);
                }
                else
                {
                    logger.LogError("Failed to create role {Role}: {Errors}", role, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }

        // Seed default Admin user
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        if (await userManager.FindByEmailAsync("admin@volunteerhub.local") == null)
        {
            var adminUser = new ApplicationUser
            {
                UserName = "admin@volunteerhub.local",
                Email = "admin@volunteerhub.local",
                FirstName = "Admin",
                LastName = "System"
            };

            var userResult = await userManager.CreateAsync(adminUser, "Admin@123!");
            if (userResult.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, AppRoles.Admin);
                logger.LogInformation("Created default admin user.");
            }
            else
            {
                logger.LogError("Failed to create default admin user: {Errors}", string.Join(", ", userResult.Errors.Select(e => e.Description)));
            }
        }
    }
}
