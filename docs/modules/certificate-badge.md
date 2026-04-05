# Module: Certificate and Badge

## Purpose
Generate electronic certificates, manage verification QR codes, award badges, and update volunteer recognition records after validated participation.

## Main actors
- Volunteer
- Organizer
- Admin
- Public verifier (limited certificate verification flow)

## Responsibilities
- Determine certificate eligibility
- Generate certificate PDF
- Generate verification QR / verification code
- Store certificate metadata
- Support public certificate verification
- Award badges based on rules
- Update volunteer passport and recognition data

## Scope
This module handles recognition after validated participation.
It depends on event completion and approved attendance, but does not itself verify attendance.

## Related modules
- Attendance
- Event Management
- Volunteer Profile
- Notification
- Admin

## Core entities
- Certificate
- CertificateTemplate (optional)
- CertificateVerification
- Badge
- VolunteerBadge
- BadgeRule
- RecognitionAuditLog

## Suggested aggregate roots
- Certificate
- Badge

## Important fields

### Certificate
- Id
- VolunteerProfileId
- EventId
- CertificateNumber
- Title
- PdfPath
- IssuedAt
- VerificationCode
- QrCodePath
- Status
- RevokedAt
- RevocationReason

### CertificateVerification
- Id
- CertificateId
- VerifiedAt
- VerificationSource
- RequestMetadata

### Badge
- Id
- Code
- Name
- Description
- IconUrl
- RuleType
- IsActive

### VolunteerBadge
- Id
- VolunteerProfileId
- BadgeId
- AwardedAt
- AwardReason
- SourceReferenceId

### BadgeRule
- Id
- BadgeId
- RuleType
- ThresholdValue
- IsEnabled

## Main use cases
1. System issues certificate after event completion and approved attendance
2. Volunteer downloads certificate
3. Third party verifies certificate via QR or verification code
4. System awards badge when threshold/rule is satisfied
5. Admin revokes invalid certificate
6. Organizer views recognition summary for participants if allowed

## Business rules
1. Certificate can be issued only for completed events with approved attendance.
2. Each eligible volunteer-event participation should produce at most one active certificate unless reissue flow exists.
3. Verification code must be unique.
4. Revoked certificates must fail verification publicly.
5. Badge awards must follow defined rule engine or threshold logic.
6. Volunteer passport and total recognition records should be updated only after successful issue/award workflows.
7. Admin can revoke certificate in fraud or correction cases.
8. Reissued certificate policy must preserve audit history.

## Validation rules
- VolunteerProfileId and EventId are required
- Eligibility must be checked before issue
- Duplicate active certificate for same participation is not allowed
- Badge code must be unique
- Revocation requires reason
- PDF path and verification code required once issued

## Public verification rules
- Public verification endpoint should reveal only safe verification info
- No private volunteer or organizer internal data should leak
- Verification response should clearly indicate valid / revoked / not found
- Verification actions may be logged for audit/analytics

## Dependencies
- Attendance for approved participation
- Event Management for event completion status
- Volunteer Profile for passport and recognition linkage
- Notification for issuing certificate email or notice
- Infrastructure PDF/QR services

## Out of scope
- Blockchain credential registry unless explicitly requested
- Rich badge gamification economy unless separately designed

## Required outputs
- Certificate and badge entities
- Eligibility service
- PDF generation abstraction
- QR generation abstraction
- Certificate verification controller
- Badge award service
- Volunteer-facing certificate query endpoints/views
- Validators
- Unit tests and integration tests

## Suggested folder targets
- src/VolunteerHub.Application/Features/Certificates/
- src/VolunteerHub.Application/Features/Badges/
- src/VolunteerHub.Infrastructure/Services/Pdf/
- src/VolunteerHub.Infrastructure/Services/QrCode/
- src/VolunteerHub.Web/Controllers/CertificateVerificationController.cs

## Suggested application services
- ICertificateService
- ICertificateEligibilityService
- IBadgeService
- IRecognitionService

## Suggested commands / operations
- IssueCertificate
- ReissueCertificate
- RevokeCertificate
- VerifyCertificate
- EvaluateBadgesForVolunteer
- AwardBadge
- GetMyCertificates

## Integration notes
- Approved attendance and event completion are mandatory prerequisites.
- Issuing a certificate may create/update volunteer passport entry.
- Badge awards should be idempotent to avoid duplicates.

## Error scenarios
- Participation not eligible
- Certificate already issued
- PDF generation failure
- QR generation failure
- Invalid verification code
- Revoked certificate lookup
- Duplicate badge award

## Testing checklist
- Eligible volunteer receives certificate
- Ineligible volunteer does not
- Verification works for active certificate
- Revoked certificate fails verification
- Badge awarded only once when threshold met
- Passport updated after successful certificate issuance