namespace VolunteerHub.Contracts.Responses;

public class VolunteerProfileResponse
{
    public Guid Id { get; set; }
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
    
    // Computed completeness flag
    public bool IsProfileComplete { get; set; }
    
    public List<string> Skills { get; set; } = new();
    public List<string> Languages { get; set; } = new();
    public List<string> Interests { get; set; } = new();
}
