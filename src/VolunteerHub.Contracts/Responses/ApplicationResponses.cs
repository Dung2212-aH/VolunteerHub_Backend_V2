namespace VolunteerHub.Contracts.Responses;

public class ApplicationResponse
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public Guid VolunteerProfileId { get; set; }
    
    public string Status { get; set; } = string.Empty;
    public DateTime AppliedAt { get; set; }
    
    public string MotivationText { get; set; } = string.Empty;
    public string AvailabilityNote { get; set; } = string.Empty;
    
    public DateTime? ReviewedAt { get; set; }
    public string RejectionReason { get; set; } = string.Empty;
}

public class ApplicantSummaryResponse : ApplicationResponse
{
    public string VolunteerFullName { get; set; } = string.Empty;
}
