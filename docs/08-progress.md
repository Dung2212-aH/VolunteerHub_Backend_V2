# Progress

## Completed Modules
- **Backend Foundation** (2026-04-04)
  - Domain base classes: `BaseEntity`, `AuditableEntity`, `ISoftDeletable`
  - Application abstractions: `Result<T>`, `Error`, `IRepository<T>`, `IUnitOfWork`
  - Contracts: `AppRoles` constants
  - Infrastructure: `AppDbContext` (Identity + audit + soft-delete), `Repository<T>`, `UnitOfWork`, `DependencyInjection`
  - Identity: `ApplicationUser`, `ApplicationRole`
  - Web: `GlobalExceptionHandler`, `Program.cs` pipeline, `appsettings.json`
  - Multi-project solution structure (5 .csproj files)
- **Auth Module** (2026-04-04)
  - Contracts: `AuthRequests.cs`
  - Application: `IAccountService.cs`
  - Infrastructure: `AccountService.cs`, `RoleSeeder.cs`
  - Web: `AccountController.cs`
- **Volunteer Profile Module** (2026-04-04)
  - Domain: `VolunteerProfile`, `VolunteerSkill`, `VolunteerLanguage`, `VolunteerInterest`
  - Contracts: `ProfileRequests.cs`, `ProfileResponses.cs`
  - Application: `IVolunteerProfileService`, `IVolunteerProfileRepository`
  - Infrastructure: `VolunteerProfileConfiguration`, `VolunteerProfileRepository`, `VolunteerProfileService`
  - Web: `VolunteerProfileController`
- **Organizer Module** (2026-04-04)
  - Domain: `OrganizerProfile`, `OrganizerVerificationStatus`
  - Contracts: `OrganizerRequests.cs`, `OrganizerResponses.cs`
  - Application: `IOrganizerService`, `IOrganizerRepository`, `IOrganizerVerificationService`
  - Infrastructure: `OrganizerProfileConfiguration`, `OrganizerProfileRepository`, `OrganizerService`, `OrganizerVerificationService`
  - Web: `OrganizerProfileController`, `OrganizerModerationController`
- **Event Management Module** (2026-04-04)
  - Domain: `Event`, `EventSkillRequirement`, `EventStatus`
  - Contracts: `EventRequests.cs`, `EventResponses.cs`
  - Application: `IEventService`, `IEventRepository`
  - Infrastructure: `EventConfiguration.cs`, `EventRepository.cs`, `EventService.cs`
  - Web: `EventsController.cs`
- **Application Approval Module** (2026-04-04)
  - Domain: `EventApplication`, `ApplicationStatus`, `ApplicationDecisionHistory`, `ApplicationReviewNote`
  - Contracts: `ApplicationRequests.cs`, `ApplicationResponses.cs`
  - Application: `IEventApplicationService`, `IApplicationReviewService`, `IApplicationApprovalRepository`
  - Infrastructure: `ApplicationApprovalRepository`, `EventApplicationConfiguration`
  - Web: `VolunteerApplicationController`, `OrganizerApplicationReviewController`
- **Attendance Module** (2026-04-04)
  - Domain: `EventShift`, `ShiftAssignment`, `AttendanceRecord`, `AttendanceStatus`, `CheckInMethod`
  - Contracts: `AttendanceRequests.cs`, `AttendanceResponses.cs`
  - Application: `IAttendanceService`, `IAttendanceRepository`
  - Infrastructure: `AttendanceRepository`, shift/attendance EF configurations
  - Web: `VolunteerAttendanceController`, `OrganizerAttendanceController`
  - Helpers: `LocationHelper` (GPS distance)
- **Certificate + Badge + QR Verification Module** (2026-04-04)
  - Domain: `Certificate`, `CertificateStatus`, `Badge`, `VolunteerBadge`
  - Contracts: `RecognitionContracts.cs`
  - Application: `ICertificateService`, `IBadgeService`, `ICertificateEligibilityService`, `IRecognitionRepository`
  - Infrastructure: `RecognitionRepository`, `CertificateEligibilityService`, EF configurations, `BadgeSeeder`
  - Web: `VolunteerCertificateController`, `AdminCertificateController`, `PublicVerificationController`
- **Notification Module** (2026-04-04)
  - Domain: `Notification`, `NotificationTemplate`, `NotificationDispatchLog`, `NotificationType`, `NotificationChannel`, `NotificationStatus`
  - Contracts: `NotificationContracts.cs`
  - Application: `INotificationService`, `INotificationRepository`, `IEmailSender`
  - Infrastructure: `NotificationRepository`, `NoOpEmailSender`, `NotificationTemplateSeeder`, EF configurations
  - Web: `NotificationController`
- **Rating & Feedback Module** (2026-04-05)
  - Domain: `Rating`, `FeedbackReport`, `RatingRole`, `ReportStatus`
  - Contracts: `RatingContracts.cs`
  - Application: `IRatingService`, `IFeedbackService`, `IRatingRepository`
  - Infrastructure: `RatingRepository`, `RatingConfiguration`
  - Web: `RatingController`, `FeedbackController`

## Bug Fixes Applied (2026-04-05)
- **Fixed UserId↔ProfileId mismatch in Volunteer controllers** — `VolunteerAttendanceController`, `VolunteerApplicationController`, `VolunteerCertificateController` now resolve `userId → profileId` via `IVolunteerProfileRepository` before calling services
- **Fixed notification triggers** — `EventApplicationService`, `ApplicationReviewService`, `CertificateService`, `BadgeService` now resolve `profileId → userId` before sending notifications
- **Added missing seeder calls** — `BadgeSeeder.SeedAsync` and `NotificationTemplateSeeder.SeedAsync` added to `Program.cs`

## In Progress
- none

## Pending Modules
- Sponsor
- Admin

## Integration Notes
- `Event.OrganizerId` == Identity UserId (confirmed from EventsController)
- `AttendanceService`, `ApplicationApprovalRepository`, `CertificateService`, `BadgeService` use `VolunteerProfileId`
- Controllers must resolve `User.GetUserId()` (Identity) → `profile.Id` (Domain) before calling services
- Notification triggers must use Identity `UserId`, not `VolunteerProfileId`
- All modules must reference `VolunteerHub.Domain` for base entities
- All modules must register services in `DependencyInjection.cs`
- All auditable entities inherit `AuditableEntity`; soft-deletable entities implement `ISoftDeletable`