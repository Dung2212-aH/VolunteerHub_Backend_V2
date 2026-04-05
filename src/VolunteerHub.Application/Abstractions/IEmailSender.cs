namespace VolunteerHub.Application.Abstractions;

public interface IEmailSender
{
    /// <summary>
    /// Sends an email. Returns a provider response string on success, or throws on failure.
    /// </summary>
    Task<string> SendEmailAsync(string toEmail, string subject, string body, CancellationToken cancellationToken = default);
}
