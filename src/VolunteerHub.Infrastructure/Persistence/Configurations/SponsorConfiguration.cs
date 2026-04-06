using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VolunteerHub.Domain.Entities;

namespace VolunteerHub.Infrastructure.Persistence.Configurations;

public class SponsorProfileConfiguration : IEntityTypeConfiguration<SponsorProfile>
{
    public void Configure(EntityTypeBuilder<SponsorProfile> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.UserId).IsUnique();
        builder.HasIndex(x => x.Status);

        builder.Property(x => x.CompanyName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Description).HasMaxLength(2000);
        builder.Property(x => x.LogoUrl).HasMaxLength(500);
        builder.Property(x => x.WebsiteUrl).HasMaxLength(500);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Phone).HasMaxLength(20);
        builder.Property(x => x.Address).HasMaxLength(500);
        builder.Property(x => x.TaxCode).HasMaxLength(50);
        builder.Property(x => x.RejectionReason).HasMaxLength(1000);

        builder.HasMany(x => x.ContactPersons)
            .WithOne(x => x.SponsorProfile)
            .HasForeignKey(x => x.SponsorProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.EventSponsors)
            .WithOne(x => x.SponsorProfile)
            .HasForeignKey(x => x.SponsorProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Contributions)
            .WithOne(x => x.SponsorProfile)
            .HasForeignKey(x => x.SponsorProfileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class SponsorContactPersonConfiguration : IEntityTypeConfiguration<SponsorContactPerson>
{
    public void Configure(EntityTypeBuilder<SponsorContactPerson> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FullName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Phone).HasMaxLength(20);
        builder.Property(x => x.Role).HasMaxLength(100);
    }
}

public class SponsorshipPackageConfiguration : IEntityTypeConfiguration<SponsorshipPackage>
{
    public void Configure(EntityTypeBuilder<SponsorshipPackage> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.EventId);
        builder.HasIndex(x => new { x.EventId, x.DisplayOrder });

        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Description).HasMaxLength(2000);
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.Benefits).HasMaxLength(4000);

        builder.HasOne(x => x.Event)
            .WithMany(x => x.SponsorshipPackages)
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class EventSponsorConfiguration : IEntityTypeConfiguration<EventSponsor>
{
    public void Configure(EntityTypeBuilder<EventSponsor> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.EventId);
        builder.HasIndex(x => x.SponsorProfileId);
        builder.HasIndex(x => x.Status);

        builder.Property(x => x.Note).HasMaxLength(1000);
        builder.Property(x => x.RejectionReason).HasMaxLength(1000);

        builder.HasOne(x => x.Event)
            .WithMany(x => x.EventSponsors)
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.SponsorshipPackage)
            .WithMany(x => x.EventSponsors)
            .HasForeignKey(x => x.SponsorshipPackageId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(x => x.Contributions)
            .WithOne(x => x.EventSponsor)
            .HasForeignKey(x => x.EventSponsorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class SponsorContributionConfiguration : IEntityTypeConfiguration<SponsorContribution>
{
    public void Configure(EntityTypeBuilder<SponsorContribution> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.EventSponsorId);
        builder.HasIndex(x => x.SponsorProfileId);

        builder.Property(x => x.Value).HasPrecision(18, 2);
        builder.Property(x => x.Description).HasMaxLength(2000);
        builder.Property(x => x.ReceiptReference).HasMaxLength(200);
        builder.Property(x => x.Note).HasMaxLength(1000);
    }
}