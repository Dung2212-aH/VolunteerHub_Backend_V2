namespace VolunteerHub.Domain.Common;

/// <summary>
/// Adds audit trail fields. AppDbContext auto-populates these on SaveChangesAsync.
/// </summary>
public abstract class AuditableEntity : BaseEntity
{
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
