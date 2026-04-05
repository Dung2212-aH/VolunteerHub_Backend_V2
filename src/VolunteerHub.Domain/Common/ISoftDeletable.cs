namespace VolunteerHub.Domain.Common;

/// <summary>
/// Marker interface for soft-deletable entities.
/// AppDbContext intercepts Delete operations and sets IsDeleted instead.
/// A global query filter automatically excludes soft-deleted records.
/// </summary>
public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
    DateTime? DeletedAt { get; set; }
}
