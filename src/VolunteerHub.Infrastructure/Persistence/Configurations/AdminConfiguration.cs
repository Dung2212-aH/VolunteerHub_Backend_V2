using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VolunteerHub.Domain.Entities;

namespace VolunteerHub.Infrastructure.Persistence.Configurations;

public class AdminActionLogConfiguration : IEntityTypeConfiguration<AdminActionLog>
{
    public void Configure(EntityTypeBuilder<AdminActionLog> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.AdminUserId);
        builder.HasIndex(x => x.ActionType);
        builder.HasIndex(x => x.EntityType);

        builder.Property(x => x.ActionType).IsRequired().HasMaxLength(100);
        builder.Property(x => x.EntityType).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).IsRequired().HasMaxLength(2000);
    }
}

public class SkillCatalogItemConfiguration : IEntityTypeConfiguration<SkillCatalogItem>
{
    public void Configure(EntityTypeBuilder<SkillCatalogItem> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.Code).IsUnique();
        builder.HasIndex(x => x.Name);

        builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Category).HasMaxLength(100);
    }
}