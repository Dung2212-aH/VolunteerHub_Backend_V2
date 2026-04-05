using VolunteerHub.Domain.Common;

namespace VolunteerHub.Domain.Entities;

public class ApplicationReviewNote : AuditableEntity
{
    public Guid EventApplicationId { get; set; }
    public EventApplication EventApplication { get; set; } = null!;

    public Guid AuthorUserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsPrivate { get; set; } = true;
}