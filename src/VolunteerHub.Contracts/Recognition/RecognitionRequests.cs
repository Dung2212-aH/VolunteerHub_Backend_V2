using System.ComponentModel.DataAnnotations;

namespace VolunteerHub.Contracts.Recognition;

public class IssueCertificateRequest
{
    [Required]
    public Guid VolunteerProfileId { get; set; }

    [Required]
    public Guid EventId { get; set; }
}

public class RevokeCertificateRequest
{
    [Required]
    public string RevocationReason { get; set; } = string.Empty;
}
