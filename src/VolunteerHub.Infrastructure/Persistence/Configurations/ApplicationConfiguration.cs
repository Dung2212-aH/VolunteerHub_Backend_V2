using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VolunteerHub.Domain.Entities;

namespace VolunteerHub.Infrastructure.Persistence.Configurations;

public class EventApplicationConfiguration : IEntityTypeConfiguration<EventApplication>
{
    public void Configure(EntityTypeBuilder<EventApplication> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasOne(e => e.Event)
            .WithMany(ev => ev.Applications)
            .HasForeignKey(e => e.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.VolunteerProfile)
            .WithMany(v => v.Applications)
            .HasForeignKey(e => e.VolunteerProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.EventId, e.VolunteerProfileId });
        builder.HasIndex(e => e.Status);

        builder.Property(e => e.MotivationText).HasMaxLength(1500);
        builder.Property(e => e.AvailabilityNote).HasMaxLength(500);
        builder.Property(e => e.RejectionReason).HasMaxLength(1000);

        builder.HasMany(e => e.ReviewNotes)
            .WithOne(n => n.EventApplication)
            .HasForeignKey(n => n.EventApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.DecisionHistory)
            .WithOne(h => h.EventApplication)
            .HasForeignKey(h => h.EventApplicationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ApplicationReviewNoteConfiguration : IEntityTypeConfiguration<ApplicationReviewNote>
{
    public void Configure(EntityTypeBuilder<ApplicationReviewNote> builder)
    {
        builder.HasKey(n => n.Id);
        builder.Property(n => n.Content).IsRequired().HasMaxLength(2000);
    }
}

public class ApplicationDecisionHistoryConfiguration : IEntityTypeConfiguration<ApplicationDecisionHistory>
{
    public void Configure(EntityTypeBuilder<ApplicationDecisionHistory> builder)
    {
        builder.HasKey(h => h.Id);
        builder.Property(h => h.Reason).HasMaxLength(1000);
    }
}
