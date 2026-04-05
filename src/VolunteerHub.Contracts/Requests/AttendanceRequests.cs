using System.ComponentModel.DataAnnotations;

namespace VolunteerHub.Contracts.Requests;

public class CheckInRequest
{
    [Required]
    public Guid EventShiftId { get; set; }
    
    public string Method { get; set; } = "GPS";
    
    [Range(-90, 90)]
    public double? Latitude { get; set; }
    
    [Range(-180, 180)]
    public double? Longitude { get; set; }
}

public class CheckOutRequest
{
    [Required]
    public Guid EventShiftId { get; set; }
    
    public string Method { get; set; } = "GPS";
    
    [Range(-90, 90)]
    public double? Latitude { get; set; }
    
    [Range(-180, 180)]
    public double? Longitude { get; set; }
}

public class ManualOverrideRequest
{
    [Required]
    public Guid VolunteerProfileId { get; set; }
    
    [Required]
    public string NewStatus { get; set; } = string.Empty;
    
    public DateTime? CheckInAt { get; set; }
    public DateTime? CheckOutAt { get; set; }
    
    [Required]
    public string Reason { get; set; } = string.Empty;
}

public class AssignShiftRequest
{
    [Required]
    public Guid VolunteerProfileId { get; set; }
}

public class CreateShiftRequest
{
    [Required]
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public DateTime StartTime { get; set; }
    
    [Required]
    public DateTime EndTime { get; set; }
    
    [Required]
    [Range(1, int.MaxValue)]
    public int MaxVolunteers { get; set; }
}
