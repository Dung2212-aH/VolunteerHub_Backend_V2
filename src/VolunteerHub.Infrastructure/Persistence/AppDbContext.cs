using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VolunteerHub.Domain.Common;
using VolunteerHub.Infrastructure.Identity;

namespace VolunteerHub.Infrastructure.Persistence;

/// <summary>
/// Central EF Core DbContext.
/// - Inherits IdentityDbContext for ASP.NET Core Identity tables.
/// - Auto-populates audit fields (CreatedAt, UpdatedAt) on save.
/// - Intercepts deletes on ISoftDeletable entities and converts to soft-delete.
/// - Applies global query filters to exclude soft-deleted records.
/// </summary>
public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // ----- Module DbSets will be added here as modules are generated -----
    public DbSet<VolunteerHub.Domain.Entities.VolunteerProfile> VolunteerProfiles => Set<VolunteerHub.Domain.Entities.VolunteerProfile>();
    public DbSet<VolunteerHub.Domain.Entities.VolunteerSkill> VolunteerSkills => Set<VolunteerHub.Domain.Entities.VolunteerSkill>();
    public DbSet<VolunteerHub.Domain.Entities.VolunteerLanguage> VolunteerLanguages => Set<VolunteerHub.Domain.Entities.VolunteerLanguage>();
    public DbSet<VolunteerHub.Domain.Entities.VolunteerInterest> VolunteerInterests => Set<VolunteerHub.Domain.Entities.VolunteerInterest>();

    public DbSet<VolunteerHub.Domain.Entities.Event> Events => Set<VolunteerHub.Domain.Entities.Event>();
    public DbSet<VolunteerHub.Domain.Entities.EventSkillRequirement> EventSkillRequirements => Set<VolunteerHub.Domain.Entities.EventSkillRequirement>();

    public DbSet<VolunteerHub.Domain.Entities.OrganizerProfile> OrganizerProfiles => Set<VolunteerHub.Domain.Entities.OrganizerProfile>();
    public DbSet<VolunteerHub.Domain.Entities.OrganizerLegalDocument> OrganizerLegalDocuments => Set<VolunteerHub.Domain.Entities.OrganizerLegalDocument>();
    public DbSet<VolunteerHub.Domain.Entities.OrganizerVerificationReview> OrganizerVerificationReviews => Set<VolunteerHub.Domain.Entities.OrganizerVerificationReview>();

    public DbSet<VolunteerHub.Domain.Entities.EventShift> EventShifts => Set<VolunteerHub.Domain.Entities.EventShift>();
    public DbSet<VolunteerHub.Domain.Entities.ShiftAssignment> ShiftAssignments => Set<VolunteerHub.Domain.Entities.ShiftAssignment>();
    public DbSet<VolunteerHub.Domain.Entities.AttendanceRecord> AttendanceRecords => Set<VolunteerHub.Domain.Entities.AttendanceRecord>();

    public DbSet<VolunteerHub.Domain.Entities.EventApplication> EventApplications => Set<VolunteerHub.Domain.Entities.EventApplication>();
    public DbSet<VolunteerHub.Domain.Entities.ApplicationReviewNote> ApplicationReviewNotes => Set<VolunteerHub.Domain.Entities.ApplicationReviewNote>();
    public DbSet<VolunteerHub.Domain.Entities.ApplicationDecisionHistory> ApplicationDecisionHistories => Set<VolunteerHub.Domain.Entities.ApplicationDecisionHistory>();

    public DbSet<VolunteerHub.Domain.Entities.Certificate> Certificates => Set<VolunteerHub.Domain.Entities.Certificate>();
    public DbSet<VolunteerHub.Domain.Entities.Badge> Badges => Set<VolunteerHub.Domain.Entities.Badge>();
    public DbSet<VolunteerHub.Domain.Entities.VolunteerBadge> VolunteerBadges => Set<VolunteerHub.Domain.Entities.VolunteerBadge>();

    public DbSet<VolunteerHub.Domain.Entities.Notification> Notifications => Set<VolunteerHub.Domain.Entities.Notification>();
    public DbSet<VolunteerHub.Domain.Entities.NotificationTemplate> NotificationTemplates => Set<VolunteerHub.Domain.Entities.NotificationTemplate>();
    public DbSet<VolunteerHub.Domain.Entities.NotificationDispatchLog> NotificationDispatchLogs => Set<VolunteerHub.Domain.Entities.NotificationDispatchLog>();

    public DbSet<VolunteerHub.Domain.Entities.Rating> Ratings => Set<VolunteerHub.Domain.Entities.Rating>();
    public DbSet<VolunteerHub.Domain.Entities.FeedbackReport> FeedbackReports => Set<VolunteerHub.Domain.Entities.FeedbackReport>();

    public DbSet<VolunteerHub.Domain.Entities.SponsorProfile> SponsorProfiles => Set<VolunteerHub.Domain.Entities.SponsorProfile>();
    public DbSet<VolunteerHub.Domain.Entities.SponsorContactPerson> SponsorContactPersons => Set<VolunteerHub.Domain.Entities.SponsorContactPerson>();
    public DbSet<VolunteerHub.Domain.Entities.SponsorContribution> SponsorContributions => Set<VolunteerHub.Domain.Entities.SponsorContribution>();
    public DbSet<VolunteerHub.Domain.Entities.EventSponsor> EventSponsors => Set<VolunteerHub.Domain.Entities.EventSponsor>();
    public DbSet<VolunteerHub.Domain.Entities.SponsorshipPackage> SponsorshipPackages => Set<VolunteerHub.Domain.Entities.SponsorshipPackage>();

    public DbSet<VolunteerHub.Domain.Entities.AdminActionLog> AdminActionLogs => Set<VolunteerHub.Domain.Entities.AdminActionLog>();
    public DbSet<VolunteerHub.Domain.Entities.SkillCatalogItem> SkillCatalogItems => Set<VolunteerHub.Domain.Entities.SkillCatalogItem>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply all IEntityTypeConfiguration<T> from this assembly
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Global query filter: automatically exclude soft-deleted entities
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
                var falseConstant = Expression.Constant(false);
                var condition = Expression.Equal(property, falseConstant);
                var lambda = Expression.Lambda(condition, parameter);

                builder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Auto-populate audit fields
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }

        // Convert hard-deletes to soft-deletes for ISoftDeletable entities
        foreach (var entry in ChangeTracker.Entries<ISoftDeletable>())
        {
            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.DeletedAt = DateTime.UtcNow;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
