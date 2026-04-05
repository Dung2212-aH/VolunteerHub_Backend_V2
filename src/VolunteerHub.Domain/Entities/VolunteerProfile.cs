using VolunteerHub.Domain.Common;

namespace VolunteerHub.Domain.Entities;

public class VolunteerProfile : AuditableEntity, ISoftDeletable
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Bio { get; set; }
    public string? BloodGroup { get; set; }
    public string? Avatar { get; set; }
    
    public int TotalVolunteerHours { get; set; }
    
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ICollection<VolunteerSkill> Skills { get; set; } = new List<VolunteerSkill>();
    public ICollection<VolunteerLanguage> Languages { get; set; } = new List<VolunteerLanguage>();
    public ICollection<VolunteerInterest> Interests { get; set; } = new List<VolunteerInterest>();
    public ICollection<EventApplication> Applications { get; set; } = new List<EventApplication>();
    public ICollection<VolunteerBadge> Badges { get; set; } = new List<VolunteerBadge>();
}
