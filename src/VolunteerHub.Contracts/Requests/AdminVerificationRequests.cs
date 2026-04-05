using System.ComponentModel.DataAnnotations;

namespace VolunteerHub.Contracts.Requests;

public class ReviewOrganizerVerificationRequest
{
    [Required]
    public string Comment { get; set; } = string.Empty;
}
