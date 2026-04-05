# Module: Auth

## Purpose
Provide authentication, authorization, account lifecycle management, and role-based access for all platform actors.

## Main actors
- Volunteer
- Organizer
- Sponsor
- Admin

## Responsibilities
- Register account
- Login
- Logout
- Email confirmation
- Forgot password
- Reset password
- Change password
- Assign default role at registration
- Role-based access checks
- Account lock/unlock support
- Profile bootstrap after registration

## Scope
This module covers platform identity and access only.
It does not manage detailed volunteer profile fields, organizer legal documents, sponsor donations, or event workflows.

## Related modules
- Volunteer Profile
- Organizer
- Sponsor
- Admin
- Notification

## Core entities
- ApplicationUser
- ApplicationRole
- RefreshSession (optional if implemented)
- EmailVerificationToken (if not using built-in Identity token provider abstraction directly)
- PasswordResetRequest (optional persistence model if needed)

## Suggested aggregate roots
- ApplicationUser

## Important fields

### ApplicationUser
- Id
- UserName
- Email
- PhoneNumber
- PasswordHash
- EmailConfirmed
- IsActive
- IsDeleted
- LastLoginAt
- CreatedAt
- UpdatedAt

### ApplicationRole
- Id
- Name
- NormalizedName

## Roles
- Volunteer
- Organizer
- Sponsor
- Admin

## Main use cases
1. User registers as Volunteer
2. User registers as Organizer
3. User registers as Sponsor
4. System assigns proper default role
5. User confirms email
6. User logs in
7. User requests password reset
8. User resets password
9. User changes password after login
10. Admin locks or disables abusive account
11. System denies unauthorized access by role/policy

## Business rules
1. Every account must have exactly one primary platform role at registration.
2. Email must be unique.
3. Email confirmation is required before sensitive actions if platform policy enables it.
4. Organizer account may register successfully before legal verification, but cannot publish events until Organizer module verification is approved.
5. Sponsor account may exist before full profile completion.
6. Disabled or locked accounts cannot log in.
7. Password must satisfy platform password policy.
8. Admin role cannot be self-assigned during public registration.
9. Registration should create minimal profile seed data for the chosen role when appropriate.
10. Failed login attempts may trigger lockout based on policy.

## Validation rules
- Email is required and must be valid format
- Password is required and must meet configured complexity
- Confirm password must match password
- Selected role must be one of allowed public roles: Volunteer, Organizer, Sponsor
- Admin registration must not be publicly exposed
- Change password requires current password
- Reset password token must be valid and not expired

## Security requirements
- Use ASP.NET Core Identity
- Store passwords only as Identity-managed hashes
- Use anti-forgery protection for MVC form posts
- Do not expose sensitive auth failure details
- Log auth security events
- Use authorization policies for Admin, Organizer, Volunteer, Sponsor sections
- Protect account management endpoints from brute-force abuse where possible

## Dependencies
- ASP.NET Core Identity
- Notification module for email confirmation and reset email
- Volunteer Profile module for bootstrap volunteer profile
- Organizer module for bootstrap organizer profile
- Sponsor module for bootstrap sponsor profile

## Out of scope
- Social login unless explicitly added later
- External OAuth providers unless explicitly requested
- Full JWT-only API auth unless architecture changes from MVC auth flow

## Required outputs
- Identity setup and configuration
- Role constants
- Authorization policies
- Register/Login/ForgotPassword/ResetPassword/ChangePassword DTOs or ViewModels
- Account service abstraction and implementation
- MVC AccountController
- AccessDenied and auth views if needed
- Seed roles
- Unit tests for account service logic
- Integration tests for auth flows

## Suggested folder targets
- src/VolunteerHub.Application/Features/Auth/
- src/VolunteerHub.Infrastructure/Identity/
- src/VolunteerHub.Web/Controllers/AccountController.cs
- src/VolunteerHub.Web/Views/Account/

## Suggested application services
- IAccountService
- IRoleBootstrapService
- ICurrentUserService

## Suggested commands / operations
- RegisterVolunteer
- RegisterOrganizer
- RegisterSponsor
- ConfirmEmail
- Login
- Logout
- ForgotPassword
- ResetPassword
- ChangePassword
- LockUser
- UnlockUser

## Integration notes
- On successful Volunteer registration, create minimal VolunteerProfile record if not present.
- On successful Organizer registration, create minimal OrganizerProfile record with verification status Pending.
- On successful Sponsor registration, create minimal SponsorProfile record if applicable.
- Notification email templates should be delegated to Notification module or infrastructure email service.

## Error scenarios
- Email already exists
- Invalid login credentials
- Email not confirmed
- Account locked out
- Invalid or expired reset token
- Unsupported registration role
- Unauthorized area access

## Testing checklist
- Registration creates correct role
- Duplicate email is rejected
- Login succeeds with valid credentials
- Login fails with invalid credentials
- Locked user cannot log in
- Reset password works with valid token
- Unauthorized user cannot access protected action
- Admin-only action rejects non-admin user