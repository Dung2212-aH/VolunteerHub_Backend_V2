# Module: Notification

## Purpose
Deliver system notifications to users through email and in-app channels for important workflow events across the platform.

## Main actors
- Volunteer
- Organizer
- Sponsor
- Admin
- System background processes if added later

## Responsibilities
- Send email notifications
- Create in-app notifications
- Manage notification templates
- Mark notifications as read
- Trigger domain event-based notification flows
- Support preference checks if notification preferences are implemented

## Scope
This module manages outbound communication and notification records.
It does not own the business decisions that cause notifications; other modules trigger them.

## Related modules
- Auth
- Organizer
- Event Management
- Application Approval
- Attendance
- Certificate and Badge
- Sponsor
- Rating and Feedback
- Admin

## Core entities
- Notification
- NotificationTemplate
- NotificationPreference (optional)
- NotificationDispatchLog
- EmailMessageQueue (optional)
- InAppNotificationReceipt (optional)

## Suggested aggregate roots
- Notification
- NotificationTemplate

## Important fields

### Notification
- Id
- UserId
- Type
- Channel
- Title
- Message
- Status
- RelatedEntityType
- RelatedEntityId
- SentAt
- ReadAt
- CreatedAt

### NotificationTemplate
- Id
- Code
- Channel
- SubjectTemplate
- BodyTemplate
- IsActive
- UpdatedAt

### NotificationPreference
- Id
- UserId
- NotificationType
- AllowEmail
- AllowInApp
- AllowSms
- UpdatedAt

### NotificationDispatchLog
- Id
- NotificationId
- Channel
- ProviderResponse
- Status
- AttemptedAt

## Notification types
- Welcome
- EmailConfirmation
- PasswordReset
- ApplicationSubmitted
- ApplicationApproved
- ApplicationRejected
- InterviewScheduled
- EventReminder
- ShiftReminder
- AttendanceIssue
- CertificateIssued
- BadgeAwarded
- DonationStatusUpdated
- ComplaintUpdated
- AdminModerationOutcome

## Channels
- Email
- InApp

## Statuses
- Pending
- Sent
- Failed
- Read
- Cancelled

## Main use cases
1. Send email confirmation after registration
2. Send password reset email
3. Notify volunteer when application is approved or rejected
4. Notify organizer when new application arrives
5. Notify volunteer before event/shift start
6. Notify volunteer when certificate is issued
7. Notify sponsor when donation status changes
8. Show in-app notification center

## Business rules
1. Business modules trigger notifications; this module handles delivery and persistence.
2. Critical notifications such as password reset and email confirmation should bypass optional marketing preferences.
3. Failed dispatch attempts should be logged.
4. In-app notifications should remain queryable until read/archived according to retention policy.
5. Notification templates should be centrally managed and reusable.
6. Sensitive raw tokens should be handled carefully in email generation flows.
7. Duplicate notification spam should be minimized where possible.

## Validation rules
- UserId required for user-targeted notification
- Type and channel required
- Template code must be unique
- Email channel requires resolvable user email
- Required template placeholders must be satisfied before send

## Security and privacy rules
- Do not leak sensitive internal details in user-facing messages
- Reset links and confirmation tokens must be handled securely
- Notification center should show only current user's notifications
- Dispatch logs may contain provider metadata and should be access-restricted

## Dependencies
- Infrastructure email service
- Auth for account-related notifications
- Application Approval for status change notifications
- Attendance for reminders or review outcomes
- Certificate and Badge for recognition notices
- Sponsor for donation updates
- Admin for moderation outcomes

## Out of scope
- SMS/push notifications unless explicitly requested
- Full background queue infrastructure unless explicitly added

## Required outputs
- Notification entities
- Template management
- Notification service
- Email sender abstraction
- In-app notification query service
- MVC controllers for notification center if applicable
- Validators
- Unit tests and integration tests

## Suggested folder targets
- src/VolunteerHub.Application/Features/Notifications/
- src/VolunteerHub.Infrastructure/Services/Email/
- src/VolunteerHub.Web/Controllers/NotificationsController.cs

## Suggested application services
- INotificationService
- IEmailSender
- INotificationTemplateService
- IInAppNotificationQueryService

## Suggested commands / operations
- SendEmailConfirmation
- SendPasswordReset
- NotifyApplicationSubmitted
- NotifyApplicationApproved
- NotifyApplicationRejected
- NotifyCertificateIssued
- NotifyBadgeAwarded
- GetMyNotifications
- MarkNotificationAsRead

## Integration notes
- Prefer event-driven trigger points from other modules to keep responsibilities clean.
- Notification templates should use stable template codes.
- In future, dispatch can be moved to background jobs without changing module contracts much.

## Error scenarios
- Missing email address
- Missing or disabled template
- Email provider failure
- Unauthorized notification read access
- Duplicate sends from repeated business action
- Broken template placeholders

## Testing checklist
- Password reset email dispatch path works
- Application approval sends correct notification
- User can read only own notifications
- Failed email attempts are logged
- Template lookup uses correct code
- Mark-as-read updates state correctly