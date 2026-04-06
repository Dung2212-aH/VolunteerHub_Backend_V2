using VolunteerHub.Domain.Common;

namespace VolunteerHub.Domain.Entities;

public class SkillCatalogItem : AuditableEntity, ISoftDeletable
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}