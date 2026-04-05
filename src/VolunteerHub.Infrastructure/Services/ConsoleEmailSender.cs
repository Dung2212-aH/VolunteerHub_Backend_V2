using VolunteerHub.Application.Abstractions;

namespace VolunteerHub.Infrastructure.Services;

/// <summary>
/// Console-based email sender stub. Replace with real SMTP/SendGrid/SES implementation.
/// </summary>
public class ConsoleEmailSender : IEmailSender
{
    public Task<string> SendEmailAsync(string toEmail, string subject, string body, CancellationToken cancellationToken = default)
    {
        // Stub: log email to console. In production, wire up an SMTP/API-based sender.
        Console.WriteLine($"[EMAIL] To: {toEmail}");
        Console.WriteLine($"[EMAIL] Subject: {subject}");
        Console.WriteLine($"[EMAIL] Body: {body[..Math.Min(body.Length, 200)]}...");
        Console.WriteLine("---");

        return Task.FromResult("ConsoleEmailSender: Delivered");
    }
}
