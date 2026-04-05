namespace VolunteerHub.Contracts.Rating;

public class CreateRatingRequest
{
    public Guid EventId { get; set; }
    public Guid TargetUserId { get; set; }
    public int Score { get; set; }
    public string? Comment { get; set; }
}

public class RatingResponse
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public Guid FromUserId { get; set; }
    public Guid ToUserId { get; set; }
    public string FromRole { get; set; } = string.Empty;
    public string ToRole { get; set; } = string.Empty;
    public int Score { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class RatingSummaryResponse
{
    public Guid TargetUserId { get; set; }
    public double AverageScore { get; set; }
    public int TotalRatings { get; set; }
}

public class CreateFeedbackReportRequest
{
    public Guid EventId { get; set; }
    public Guid TargetUserId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class FeedbackReportResponse
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public Guid ReporterUserId { get; set; }
    public Guid TargetUserId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
}
