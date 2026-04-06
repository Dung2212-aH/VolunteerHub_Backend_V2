using System.ComponentModel.DataAnnotations;

namespace VolunteerHub.Contracts.Requests;

public class CreateSkillCatalogItemRequest
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    public int SortOrder { get; set; }
}

public class UpdateSkillCatalogItemRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
}

public class ResolveFeedbackReportRequest
{
    [Required]
    public bool Approve { get; set; }

    [Required]
    [MaxLength(1000)]
    public string ResolutionNote { get; set; } = string.Empty;
}