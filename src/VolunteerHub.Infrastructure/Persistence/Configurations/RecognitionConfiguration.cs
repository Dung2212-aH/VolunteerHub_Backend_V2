using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VolunteerHub.Domain.Entities;

namespace VolunteerHub.Infrastructure.Persistence.Configurations;

public class CertificateConfiguration : IEntityTypeConfiguration<Certificate>
{
    public void Configure(EntityTypeBuilder<Certificate> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasIndex(e => e.VerificationCode).IsUnique();
        
        // Ensure no multiple active certificates per volunteer + event. 
        // A simple composite index might block if we don't handle Revoked cleanly, but for now we enforce unique constraint on pairs.
        // If a request for full re-issue is strictly added later, this index could be adjusted. Currently locked: "no existing active certificates".
        builder.HasIndex(e => new { e.VolunteerProfileId, e.EventId }).IsUnique();

        builder.Property(e => e.CertificateNumber).HasMaxLength(100);
        builder.Property(e => e.Title).HasMaxLength(200);
        builder.Property(e => e.VerificationCode).HasMaxLength(255);
        builder.Property(e => e.QrCodeContent).HasMaxLength(1000);
        builder.Property(e => e.PdfPath).HasMaxLength(2000);
    }
}

public class BadgeConfiguration : IEntityTypeConfiguration<Badge>
{
    public void Configure(EntityTypeBuilder<Badge> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.Code).IsUnique();
        builder.Property(e => e.Code).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
    }
}

public class VolunteerBadgeConfiguration : IEntityTypeConfiguration<VolunteerBadge>
{
    public void Configure(EntityTypeBuilder<VolunteerBadge> builder)
    {
        builder.HasKey(e => e.Id);
        
        // Unique VolunteerBadge per volunteer + badge
        builder.HasIndex(e => new { e.VolunteerProfileId, e.BadgeId }).IsUnique();

        builder.Property(e => e.SourceReference).HasMaxLength(500);
        
        builder.HasOne(e => e.Badge)
            .WithMany()
            .HasForeignKey(e => e.BadgeId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(e => e.VolunteerProfile)
            .WithMany(v => v.Badges)
            .HasForeignKey(e => e.VolunteerProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
