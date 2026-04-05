using VolunteerHub.Domain.Common;
using VolunteerHub.Domain.Enums;

namespace VolunteerHub.Domain.Entities;

public class Rating : BaseEntity
{
    public Guid EventId { get; set; }
    public Guid FromUserId { get; set; }
    public Guid ToUserId { get; set; }
    public RatingRole FromRole { get; set; }
    public RatingRole ToRole { get; set; }
    public int Score { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
