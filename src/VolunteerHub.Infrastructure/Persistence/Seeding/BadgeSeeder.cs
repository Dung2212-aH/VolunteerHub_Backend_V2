using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VolunteerHub.Domain.Entities;
using VolunteerHub.Infrastructure.Persistence;

namespace VolunteerHub.Infrastructure.Persistence.Seeding;

public static class BadgeSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var badges = new List<Badge>
        {
            new() { Code = "FIRST_STEP", Name = "First Step", Description = "Awarded for completing the first volunteer event.", IsActive = true },
            new() { Code = "HELPING_HAND", Name = "Helping Hand", Description = "Awarded for completing 3 valid volunteer events.", IsActive = true },
            new() { Code = "CONSISTENT_VOLUNTEER", Name = "Consistent Volunteer", Description = "Awarded for reaching 20 approved volunteer hours.", IsActive = true },
            new() { Code = "COMMUNITY_CHAMPION", Name = "Community Champion", Description = "Awarded for completing 10 valid volunteer events.", IsActive = true }
        };

        foreach (var badge in badges)
        {
            var existing = await context.Badges.FirstOrDefaultAsync(b => b.Code == badge.Code);
            if (existing == null)
            {
                context.Badges.Add(badge);
            }
        }

        await context.SaveChangesAsync();
    }
}
