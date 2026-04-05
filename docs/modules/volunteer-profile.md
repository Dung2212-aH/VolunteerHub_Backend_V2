# Module: Volunteer Profile

## Purpose
Manage detailed volunteer identity, skills, preferences, portfolio, volunteer passport, and contribution history.

## Main actors
- Volunteer
- Organizer (read-limited access when evaluating applicants)
- Admin

## Responsibilities
- Create and update volunteer profile
- Manage skills and proficiency
- Manage languages
- Manage interests and causes
- Store blood group and emergency details if permitted
- Track volunteer passport history
- Track total volunteer hours
- Expose profile summary for event application review
- Provide public/private visibility controls for profile sections if needed

## Scope
This module manages volunteer-specific profile data and contribution records.
It does not approve event applications, perform check-in, or generate certificates directly.

## Related modules
- Auth
- Application Approval
- Attendance
- Certificate and Badge
- Rating and Feedback

## Core entities
- VolunteerProfile
- VolunteerSkill
- VolunteerLanguage
- VolunteerInterest
- VolunteerPortfolioItem
- VolunteerPassportEntry
- VolunteerHourLedger
- EmergencyContact (optional)
- VolunteerAvailability (optional)

## Suggested aggregate roots
- VolunteerProfile

## Important fields

### VolunteerProfile
- Id
- UserId
- FullName
- DateOfBirth (optional depending scope)
- Gender (optional if needed)
- Phone
- Address
- Latitude
- Longitude
- Bio
- BloodGroup
- AvatarUrl
- IsProfileComplete
- VisibilityLevel
- CreatedAt
- UpdatedAt

### VolunteerSkill
- Id
- VolunteerProfileId
- SkillId
- SkillNameSnapshot
- ProficiencyLevel
- YearsOfExperience
- Notes

### VolunteerLanguage
- Id
- VolunteerProfileId
- LanguageCode
- LanguageName
- ProficiencyLevel

### VolunteerInterest
- Id
- VolunteerProfileId
- InterestName

### VolunteerPortfolioItem
- Id
- VolunteerProfileId
- Title
- Description
- Url
- AttachmentPath
- IssuedBy
- OccurredAt

### VolunteerPassportEntry
- Id
- VolunteerProfileId
- EventId
- EventTitleSnapshot
- OrganizerNameSnapshot
- HoursEarned
- ParticipationStatus
- CertificateId
- CompletedAt

### VolunteerHourLedger
- Id
- VolunteerProfileId
- SourceType
- SourceReferenceId
- HoursDelta
- Description
- EffectiveAt

## Main use cases
1. Volunteer completes initial profile after registration
2. Volunteer updates skills and languages
3. Volunteer adds portfolio items or experience notes
4. Organizer views volunteer summary during application review
5. System shows volunteer passport of past completed events
6. System calculates total volunteer hours
7. Admin reviews suspicious or abusive profile data
8. Volunteer updates availability and preferences if this feature is enabled

## Business rules
1. Every volunteer account should have at most one active VolunteerProfile.
2. Total volunteer hours must be derived from approved participation records or controlled ledger entries.
3. Organizer can view only application-relevant volunteer profile information, not unrestricted private data.
4. Volunteer passport entries should be created only from completed and validated participation workflows.
5. Profile completeness should be derived from mandatory field completion rules.
6. Skill catalog references should use standardized skill definitions when available.
7. Admin can moderate or hide inappropriate portfolio items.
8. Profile visibility rules must be respected when data is exposed outside the volunteer’s own account.

## Validation rules
- FullName is required
- Phone format must be valid if provided
- BloodGroup must be from allowed enum if used
- Skill proficiency must be within allowed values
- Duplicate identical skill entries should be prevented
- Duplicate language entries should be prevented
- Portfolio URL must be valid if provided
- Hours cannot be directly edited by volunteer unless explicitly allowed by admin workflow

## Privacy and security rules
- Personally sensitive data should be minimized
- Private data must not be shown to organizers beyond defined review scope
- Emergency contact fields must be restricted
- Hours and passport history changes must be auditable
- Profile ownership must be enforced by current user identity

## Dependencies
- Auth for current user identity
- Application Approval for organizer-facing applicant profile summary
- Attendance for approved participation
- Certificate and Badge for certificate-linked passport entries
- Admin for moderation controls

## Out of scope
- Medical diagnosis handling
- Background verification unless separately defined
- External resume parsing unless separately requested

## Required outputs
- VolunteerProfile entities and related child entities
- EF Core configurations
- Repository interfaces and implementations
- Profile service and query service
- DTOs/ViewModels for create/update/read
- MVC controller(s) for volunteer self-service
- Read model for organizer applicant review
- Validation rules
- Unit tests
- Integration tests for ownership and visibility

## Suggested folder targets
- src/VolunteerHub.Application/Features/Volunteers/
- src/VolunteerHub.Domain/Entities/
- src/VolunteerHub.Infrastructure/Persistence/Configurations/
- src/VolunteerHub.Web/Areas/Volunteer/Controllers/ProfileController.cs

## Suggested application services
- IVolunteerProfileService
- IVolunteerPassportService
- IVolunteerHoursService
- IVolunteerSkillCatalogService (if shared catalog is inside this module initially)

## Suggested commands / operations
- CreateProfile
- UpdateProfile
- AddSkill
- RemoveSkill
- AddLanguage
- RemoveLanguage
- AddInterest
- RemoveInterest
- AddPortfolioItem
- RemovePortfolioItem
- GetMyProfile
- GetVolunteerSummaryForApplication
- RecalculateVolunteerHours

## Integration notes
- On first login after registration, user may be redirected to complete profile.
- Application Approval should consume a volunteer summary projection, not direct unrestricted entity access.
- Attendance and Certificate modules should write ledger/passport records through controlled service methods.

## Error scenarios
- Profile not found for authenticated volunteer
- User tries to edit another volunteer’s profile
- Duplicate skill or language
- Invalid skill catalog entry
- Organizer tries to access private fields
- Hours recalculation mismatch

## Testing checklist
- Volunteer can create and update own profile
- Another volunteer cannot edit someone else’s profile
- Organizer can read allowed summary fields only
- Duplicate skills are rejected
- Passport entries reflect approved event completion
- Total hours equals accumulated approved ledger entries