using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VolunteerHub.Domain.Entities;

namespace VolunteerHub.Infrastructure.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.Type);
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.CreatedAt);
        builder.HasIndex(e => new { e.UserId, e.Status });

        builder.Property(e => e.Title).IsRequired().HasMaxLength(500);
        builder.Property(e => e.Message).IsRequired().HasMaxLength(4000);
        builder.Property(e => e.RelatedEntityType).HasMaxLength(100);

        builder.HasMany(e => e.DispatchLogs)
            .WithOne(d => d.Notification)
            .HasForeignKey(d => d.NotificationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class NotificationTemplateConfiguration : IEntityTypeConfiguration<NotificationTemplate>
{
    public void Configure(EntityTypeBuilder<NotificationTemplate> builder)
    {
        builder.HasKey(e => e.Id);

        // Unique on Code + Channel so we can have InApp and Email templates per code
        builder.HasIndex(e => new { e.Code, e.Channel }).IsUnique();

        builder.Property(e => e.Code).IsRequired().HasMaxLength(100);
        builder.Property(e => e.SubjectTemplate).IsRequired().HasMaxLength(500);
        builder.Property(e => e.BodyTemplate).IsRequired().HasMaxLength(4000);
    }
}

public class NotificationDispatchLogConfiguration : IEntityTypeConfiguration<NotificationDispatchLog>
{
    public void Configure(EntityTypeBuilder<NotificationDispatchLog> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasIndex(e => e.NotificationId);

        builder.Property(e => e.ProviderResponse).HasMaxLength(2000);
    }
}
