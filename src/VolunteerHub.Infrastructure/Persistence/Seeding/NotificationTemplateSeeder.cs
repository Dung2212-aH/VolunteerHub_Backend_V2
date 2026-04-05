using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VolunteerHub.Domain.Entities;
using VolunteerHub.Domain.Enums;
using VolunteerHub.Infrastructure.Persistence;

namespace VolunteerHub.Infrastructure.Persistence.Seeding;

public static class NotificationTemplateSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var templates = new List<NotificationTemplate>
        {
            // ── Welcome ─────────────────────────────────────────────
            new() { Code = "WELCOME", Channel = NotificationChannel.InApp,
                SubjectTemplate = "Welcome to VolunteerHub!",
                BodyTemplate = "Hi {{FirstName}}, welcome to VolunteerHub! Start exploring volunteer opportunities today." },
            new() { Code = "WELCOME", Channel = NotificationChannel.Email,
                SubjectTemplate = "Welcome to VolunteerHub, {{FirstName}}!",
                BodyTemplate = "Hi {{FirstName}},\n\nWelcome to VolunteerHub! We're excited to have you.\n\nStart exploring volunteer opportunities and make a difference in your community." },

            // ── Email Confirmation ──────────────────────────────────
            new() { Code = "EMAIL_CONFIRMATION", Channel = NotificationChannel.Email,
                SubjectTemplate = "Confirm your email address",
                BodyTemplate = "Please confirm your email address by clicking the link below:\n\n{{ConfirmationLink}}\n\nIf you did not create an account, please ignore this email." },

            // ── Password Reset ──────────────────────────────────────
            new() { Code = "PASSWORD_RESET", Channel = NotificationChannel.Email,
                SubjectTemplate = "Reset your password",
                BodyTemplate = "You requested a password reset. Click the link below to set a new password:\n\n{{ResetLink}}\n\nIf you did not request this, please ignore this email." },

            // ── Application Submitted ───────────────────────────────
            new() { Code = "APPLICATION_SUBMITTED", Channel = NotificationChannel.InApp,
                SubjectTemplate = "Application Submitted",
                BodyTemplate = "Your application for \"{{EventTitle}}\" has been submitted successfully. You will be notified when it is reviewed." },

            // ── Application Approved ────────────────────────────────
            new() { Code = "APPLICATION_APPROVED", Channel = NotificationChannel.InApp,
                SubjectTemplate = "Application Approved",
                BodyTemplate = "Congratulations! Your application for \"{{EventTitle}}\" has been approved. Check the event details for next steps." },

            // ── Application Rejected ────────────────────────────────
            new() { Code = "APPLICATION_REJECTED", Channel = NotificationChannel.InApp,
                SubjectTemplate = "Application Not Approved",
                BodyTemplate = "Your application for \"{{EventTitle}}\" was not approved. Reason: {{Reason}}" },

            // ── Certificate Issued ──────────────────────────────────
            new() { Code = "CERTIFICATE_ISSUED", Channel = NotificationChannel.InApp,
                SubjectTemplate = "Certificate Issued",
                BodyTemplate = "A certificate has been issued for your participation in \"{{EventTitle}}\". View it in your certificates section." },

            // ── Badge Awarded ───────────────────────────────────────
            new() { Code = "BADGE_AWARDED", Channel = NotificationChannel.InApp,
                SubjectTemplate = "New Badge Earned!",
                BodyTemplate = "Congratulations! You have earned the \"{{BadgeName}}\" badge. Keep up the great work!" },
        };

        foreach (var template in templates)
        {
            var exists = await context.NotificationTemplates
                .AnyAsync(t => t.Code == template.Code && t.Channel == template.Channel);
            if (!exists)
            {
                context.NotificationTemplates.Add(template);
            }
        }

        await context.SaveChangesAsync();
    }
}
