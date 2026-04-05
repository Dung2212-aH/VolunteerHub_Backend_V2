namespace VolunteerHub.Application.Common;

/// <summary>
/// Represents a domain/application error with a code and human-readable message.
/// </summary>
public sealed record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NullValue = new("Error.NullValue", "A null value was provided.");
    public static readonly Error NotFound = new("Error.NotFound", "The requested resource was not found.");
    public static readonly Error ValidationError = new("Error.Validation", "A validation error occurred.");
    public static readonly Error Conflict = new("Error.Conflict", "A conflict occurred.");
    public static readonly Error Unauthorized = new("Error.Unauthorized", "Unauthorized access.");
    public static readonly Error Forbidden = new("Error.Forbidden", "Access is forbidden.");
    
    // Auth Errors
    public static readonly Error InvalidCredentials = new("Auth.InvalidCredentials", "Invalid email or password.");
    public static readonly Error UserRegistrationFailed = new("Auth.UserRegistrationFailed", "User registration failed.");
    public static readonly Error PasswordChangeFailed = new("Auth.PasswordChangeFailed", "Failed to change password.");

    // Profile Errors
    public static readonly Error ProfileAlreadyExists = new("Profile.AlreadyExists", "User already has a volunteer profile.");
}
