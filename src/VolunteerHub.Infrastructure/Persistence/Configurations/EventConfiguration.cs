using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VolunteerHub.Domain.Entities;

namespace VolunteerHub.Infrastructure.Persistence.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.HasIndex(e => e.OrganizerId);
        builder.HasIndex(e => e.Status);

        builder.Property(e => e.Title).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Description).IsRequired().HasMaxLength(2000);
        builder.Property(e => e.Address).IsRequired().HasMaxLength(500);

        builder.HasMany(e => e.SkillRequirements)
            .WithOne(s => s.Event)
            .HasForeignKey(s => s.EventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class EventSkillRequirementConfiguration : IEntityTypeConfiguration<EventSkillRequirement>
{
    public void Configure(EntityTypeBuilder<EventSkillRequirement> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.SkillName).IsRequired().HasMaxLength(100);
        builder.HasIndex(s => new { s.EventId, s.SkillName }).IsUnique();
    }
}
