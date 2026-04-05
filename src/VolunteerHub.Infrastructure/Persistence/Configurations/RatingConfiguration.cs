using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VolunteerHub.Domain.Entities;

namespace VolunteerHub.Infrastructure.Persistence.Configurations;

public class RatingConfiguration : IEntityTypeConfiguration<Rating>
{
    public void Configure(EntityTypeBuilder<Rating> builder)
    {
        builder.HasKey(e => e.Id);

        // Unique constraint: one rating per (EventId, FromUserId, ToUserId)
        builder.HasIndex(e => new { e.EventId, e.FromUserId, e.ToUserId }).IsUnique();

        builder.HasIndex(e => e.EventId);
        builder.HasIndex(e => e.FromUserId);
        builder.HasIndex(e => e.ToUserId);

        builder.Property(e => e.Score).IsRequired();
        builder.Property(e => e.Comment).HasMaxLength(2000);
    }
}

public class FeedbackReportConfiguration : IEntityTypeConfiguration<FeedbackReport>
{
    public void Configure(EntityTypeBuilder<FeedbackReport> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasIndex(e => e.EventId);
        builder.HasIndex(e => e.ReporterUserId);
        builder.HasIndex(e => e.TargetUserId);

        builder.Property(e => e.Reason).IsRequired().HasMaxLength(500);
        builder.Property(e => e.Description).HasMaxLength(4000);
    }
}
