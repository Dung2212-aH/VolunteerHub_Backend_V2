using System.ComponentModel.DataAnnotations;

namespace VolunteerHub.Contracts.Requests;

public class ApplyToEventRequest
{
    [Required]
    public Guid EventId { get; set; }
    
    [MaxLength(1500)]
    public string MotivationText { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string AvailabilityNote { get; set; } = string.Empty;
}

public class WithdrawApplicationRequest
{
}

public class ReviewApplicationRequest
{
    public string Reason { get; set; } = string.Empty;
}

public class AddReviewNoteRequest
{
    [Required]
    [MaxLength(2000)]
    public string Content { get; set; } = string.Empty;
    
    public bool IsPrivate { get; set; } = true;
}
