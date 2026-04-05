using Microsoft.AspNetCore.Identity;

namespace VolunteerHub.Infrastructure.Identity;

/// <summary>
/// Application role with an optional description field.
/// </summary>
public class ApplicationRole : IdentityRole<Guid>
{
    public string? Description { get; set; }

    public ApplicationRole() : base() { }

    public ApplicationRole(string roleName) : base(roleName) { }

    public ApplicationRole(string roleName, string description) : base(roleName)
    {
        Description = description;
    }
}
