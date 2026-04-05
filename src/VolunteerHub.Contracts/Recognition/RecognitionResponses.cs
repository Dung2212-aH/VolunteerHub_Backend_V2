namespace VolunteerHub.Contracts.Recognition;

public class CertificateResponse
{
    public Guid Id { get; set; }
    public Guid VolunteerProfileId { get; set; }
    public Guid EventId { get; set; }
    public string CertificateNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; }
    public string VerificationCode { get; set; } = string.Empty;
    public string QrCodeContent { get; set; } = string.Empty;
    public string? PdfPath { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class CertificateVerificationResponse
{
    public bool IsValid { get; set; }
    public string CertificateNumber { get; set; } = string.Empty;
    public string VolunteerDisplayName { get; set; } = string.Empty;
    public string EventTitle { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class BadgeResponse
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime AwardedAt { get; set; }
    public string AwardReason { get; set; } = string.Empty;
}
