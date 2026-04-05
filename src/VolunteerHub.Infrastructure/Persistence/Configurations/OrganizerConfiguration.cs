using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VolunteerHub.Domain.Entities;

namespace VolunteerHub.Infrastructure.Persistence.Configurations;

public class OrganizerProfileConfiguration : IEntityTypeConfiguration<OrganizerProfile>
{
    public void Configure(EntityTypeBuilder<OrganizerProfile> builder)
    {
        builder.HasKey(o => o.Id);
        
        builder.HasIndex(o => o.UserId).IsUnique();
        builder.HasIndex(o => o.VerificationStatus);

        builder.Property(o => o.OrganizationName).IsRequired().HasMaxLength(200);
        builder.Property(o => o.Email).IsRequired().HasMaxLength(150);

        builder.HasMany(o => o.LegalDocuments)
            .WithOne(d => d.OrganizerProfile)
            .HasForeignKey(d => d.OrganizerProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.VerificationReviews)
            .WithOne(r => r.OrganizerProfile)
            .HasForeignKey(r => r.OrganizerProfileId)
            .OnDelete(DeleteBehavior.Cascade);            
    }
}

public class OrganizerLegalDocumentConfiguration : IEntityTypeConfiguration<OrganizerLegalDocument>
{
    public void Configure(EntityTypeBuilder<OrganizerLegalDocument> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.DocumentType).IsRequired().HasMaxLength(100);
        builder.Property(d => d.FilePath).IsRequired().HasMaxLength(500);
    }
}
