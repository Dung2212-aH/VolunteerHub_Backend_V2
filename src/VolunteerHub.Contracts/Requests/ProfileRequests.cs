using System.ComponentModel.DataAnnotations;

namespace VolunteerHub.Contracts.Requests;

public class CreateProfileRequest
{
    [Required]
    public string FullName { get; set; } = string.Empty;
    
    [Required]
    public string Phone { get; set; } = string.Empty;
    
    [Required]
    public string Address { get; set; } = string.Empty;
    
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    
    public string? Bio { get; set; }
    public string? BloodGroup { get; set; }
    public string? Avatar { get; set; }

    public List<string> Skills { get; set; } = new();
    public List<string> Languages { get; set; } = new();
    public List<string> Interests { get; set; } = new();
}

public class UpdateProfileRequest
{
    [Required]
    public string FullName { get; set; } = string.Empty;
    
    [Required]
    public string Phone { get; set; } = string.Empty;
    
    [Required]
    public string Address { get; set; } = string.Empty;
    
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    
    public string? Bio { get; set; }
    public string? BloodGroup { get; set; }
    public string? Avatar { get; set; }

    public List<string> Skills { get; set; } = new();
    public List<string> Languages { get; set; } = new();
    public List<string> Interests { get; set; } = new();
}
