using VolunteerHub.Domain.Common;

namespace VolunteerHub.Domain.Entities;

public class SponsorContactPerson : BaseEntity
{
    public Guid SponsorProfileId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Role { get; set; }

    // Navigation
    public SponsorProfile SponsorProfile { get; set; } = null!;
}
