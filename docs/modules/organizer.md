# Module: Organizer

## Purpose
Manage organizer identity, organization profile, legal verification, operational credibility, and ownership over events.

## Main actors
- Organizer
- Admin
- Volunteer (public read-only trust view)
- Sponsor (limited trust view)

## Responsibilities
- Create and maintain organizer profile
- Upload and manage legal verification documents
- Store organization mission and contact information
- Track verification status
- Track public credibility/trust indicators
- Expose organization info for public event pages
- Control ownership over events created by the organizer

## Scope
This module handles organizer organization data and verification state.
It does not create events directly, but Event Management depends on organizer verification and ownership data.

## Related modules
- Auth
- Event Management
- Rating and Feedback
- Admin
- Sponsor

## Core entities
- OrganizerProfile
- OrganizerLegalDocument
- OrganizerVerificationReview
- OrganizerContact
- OrganizationCategory (optional)
- OrganizerPublicStats (optional projection/read model)

## Suggested aggregate roots
- OrganizerProfile

## Important fields

### OrganizerProfile
- Id
- UserId
- OrganizationName
- OrganizationType
- RegistrationNumber
- TaxCode (optional)
- Description
- Mission
- WebsiteUrl
- LogoUrl
- Email
- Phone
- Address
- Latitude
- Longitude
- VerificationStatus
- VerifiedAt
- RejectionReason
- CreatedAt
- UpdatedAt

### OrganizerLegalDocument
- Id
- OrganizerProfileId
- DocumentType
- FilePath
- OriginalFileName
- Status
- UploadedAt
- ReviewedAt
- ReviewerId
- Notes

### OrganizerVerificationReview
- Id
- OrganizerProfileId
- PreviousStatus
- NewStatus
- ReviewerId
- Comment
- ReviewedAt

## Main use cases
1. Organizer completes organization profile
2. Organizer uploads legal documents
3. Admin reviews and approves/rejects organizer verification
4. Public users view organization overview and trust status
5. Event module checks whether organizer is verified before publish
6. Admin audits organizer verification history

## Business rules
1. Each organizer user account should have at most one active OrganizerProfile.
2. Organizer can create draft events before verification if allowed by policy, but cannot publish until verified.
3. Verification status is controlled by Admin only.
4. Legal documents must be reviewable and auditable.
5. Rejected organizer can re-submit corrected documents.
6. Suspended organizer must not publish new events.
7. Public event pages should expose trust-relevant organizer details without revealing sensitive admin-only verification notes.

## Verification statuses
- Pending
- UnderReview
- Approved
- Rejected
- Suspended

## Validation rules
- OrganizationName is required
- Email is required and valid
- At least one contact method is required
- Required legal document types must be present before admin review can move to Approved
- Website URL must be valid if provided
- RegistrationNumber format may be validated if local policy is defined
- Verification status changes must be authorized

## Security and moderation rules
- Only organizer owner or admin can edit organizer profile
- Only admin can approve/reject/suspend
- Document access should be restricted
- Verification history must be auditable
- Sensitive legal files must not be publicly accessible directly by URL without authorization

## Dependencies
- Auth for user identity and organizer role
- Admin for verification workflow
- Event Management for ownership and publish permission
- Rating and Feedback for trust indicators
- Sponsor for sponsor-facing organization credibility

## Out of scope
- External government registry integration unless explicitly requested
- Financial auditing of organizer unless added later

## Required outputs
- Organizer entities
- Verification status enum
- EF Core mappings
- Organizer service
- Verification review service
- File upload integration abstraction
- MVC controllers for organizer self-management and admin review
- Public read model for organization trust overview
- Validators
- Unit tests and integration tests

## Suggested folder targets
- src/VolunteerHub.Application/Features/Organizers/
- src/VolunteerHub.Web/Areas/Organizer/Controllers/
- src/VolunteerHub.Web/Areas/Admin/Controllers/OrganizerModerationController.cs

## Suggested application services
- IOrganizerProfileService
- IOrganizerVerificationService
- IOrganizerPublicQueryService

## Suggested commands / operations
- CreateOrganizerProfile
- UpdateOrganizerProfile
- UploadLegalDocument
- SubmitForVerification
- ApproveOrganizer
- RejectOrganizer
- SuspendOrganizer
- GetOrganizerPublicProfile

## Integration notes
- Event publish flow must query organizer verification status.
- Public event details should display organizer trust data from this module.
- Sponsor views may consume organizer public trust summary.

## Error scenarios
- Organizer profile missing
- Unauthorized profile edit
- Required documents missing
- Non-admin trying to review verification
- Suspended organizer attempting to publish event
- Illegal status transition

## Testing checklist
- Organizer can update own profile
- Admin can approve or reject verification
- Non-admin cannot change verification status
- Unverified organizer cannot publish event
- Rejected organizer can resubmit documents
- Public profile excludes private review comments