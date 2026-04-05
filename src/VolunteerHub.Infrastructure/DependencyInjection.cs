using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VolunteerHub.Application.Abstractions;
using VolunteerHub.Infrastructure.Identity;
using VolunteerHub.Infrastructure.Persistence;
using VolunteerHub.Infrastructure.Persistence.Repositories;

namespace VolunteerHub.Infrastructure;

/// <summary>
/// Registers all Infrastructure-layer services into the DI container.
/// Called from Program.cs via builder.Services.AddInfrastructure(config).
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ── Database ────────────────────────────────────────────────
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        // ── Identity ────────────────────────────────────────────────
        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 8;

            options.User.RequireUniqueEmail = true;

            options.SignIn.RequireConfirmedEmail = false;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        // ── Repositories ────────────────────────────────────────────
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IVolunteerProfileRepository, VolunteerProfileRepository>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IOrganizerRepository, OrganizerRepository>();
        services.AddScoped<IAttendanceRepository, AttendanceRepository>();
        services.AddScoped<IApplicationApprovalRepository, ApplicationApprovalRepository>();
        services.AddScoped<IRecognitionRepository, RecognitionRepository>();

        // ── Services ────────────────────────────────────────────────
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<VolunteerHub.Application.Abstractions.IVolunteerProfileService, VolunteerHub.Application.Services.VolunteerProfileService>();
        services.AddScoped<VolunteerHub.Application.Abstractions.IEventService, VolunteerHub.Application.Services.EventService>();
        services.AddScoped<VolunteerHub.Application.Abstractions.IOrganizerProfileService, VolunteerHub.Application.Services.OrganizerProfileService>();
        services.AddScoped<VolunteerHub.Application.Abstractions.IOrganizerVerificationService, VolunteerHub.Application.Services.OrganizerVerificationService>();
        services.AddScoped<VolunteerHub.Application.Abstractions.IAttendanceService, VolunteerHub.Application.Services.AttendanceService>();
        services.AddScoped<VolunteerHub.Application.Abstractions.IEventApplicationService, VolunteerHub.Application.Services.EventApplicationService>();
        services.AddScoped<VolunteerHub.Application.Abstractions.IApplicationReviewService, VolunteerHub.Application.Services.ApplicationReviewService>();

        services.AddScoped<VolunteerHub.Application.Abstractions.ICertificateEligibilityService, VolunteerHub.Application.Services.CertificateEligibilityService>();
        services.AddScoped<VolunteerHub.Application.Abstractions.ICertificateService, VolunteerHub.Application.Services.CertificateService>();
        services.AddScoped<VolunteerHub.Application.Abstractions.IBadgeService, VolunteerHub.Application.Services.BadgeService>();

        // ── Notification ─────────────────────────────────────────────
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IEmailSender, VolunteerHub.Infrastructure.Services.ConsoleEmailSender>();
        services.AddScoped<INotificationService, VolunteerHub.Application.Services.NotificationService>();

        // ── Rating & Feedback ────────────────────────────────────────
        services.AddScoped<IRatingRepository, RatingRepository>();
        services.AddScoped<IRatingService, VolunteerHub.Application.Services.RatingService>();
        services.AddScoped<IFeedbackService, VolunteerHub.Application.Services.FeedbackService>();

        return services;
    }
}
