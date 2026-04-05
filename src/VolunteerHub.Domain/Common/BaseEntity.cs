namespace VolunteerHub.Domain.Common;

/// <summary>
/// Base entity with a GUID primary key. All domain entities inherit from this.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; }
}
