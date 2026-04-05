# Module: Attendance

## Purpose
Record volunteer participation at event time through QR or GPS-based check-in/check-out, manage shifts, and produce validated attendance hours.

## Main actors
- Volunteer
- Organizer
- Admin

## Responsibilities
- Assign volunteers to shifts
- Generate attendance tokens/QR payload if needed
- Check in by QR
- Check in by GPS proximity
- Check out
- Mark late/absent/manual override
- Validate earned hours
- Provide attendance reports

## Scope
This module covers operational participation tracking during and after an event.
It does not accept applications or issue certificates directly, but it feeds those modules.

## Related modules
- Event Management
- Application Approval
- Certificate and Badge
- Volunteer Profile
- Notification

## Core entities
- EventShift
- ShiftAssignment
- AttendanceRecord
- AttendanceVerification
- AttendanceIssue
- AttendanceAuditLog

## Suggested aggregate roots
- AttendanceRecord
- EventShift

## Important fields

### EventShift
- Id
- EventId
- Name
- RoleName
- StartsAt
- EndsAt
- Capacity
- LocationName
- Latitude
- Longitude

### ShiftAssignment
- Id
- EventShiftId
- VolunteerProfileId
- EventApplicationId
- AssignedAt
- AssignedByUserId
- Status

### AttendanceRecord
- Id
- EventId
- EventShiftId
- VolunteerProfileId
- CheckInAt
- CheckOutAt
- CheckInMethod
- CheckOutMethod
- CheckInLatitude
- CheckInLongitude
- CheckOutLatitude
- CheckOutLongitude
- Status
- ApprovedHours
- ReviewedByUserId
- ReviewedAt

### AttendanceVerification
- Id
- AttendanceRecordId
- VerificationType
- VerificationValue
- IsSuccessful
- VerifiedAt

### AttendanceIssue
- Id
- AttendanceRecordId
- IssueType
- Description
- CreatedByUserId
- CreatedAt
- ResolutionStatus

## Statuses
- Pending
- CheckedIn
- CheckedOut
- Late
- Absent
- NeedsReview
- Approved
- Rejected

## Check methods
- QR
- GPS
- Manual

## Main use cases
1. Organizer creates event shifts
2. Organizer assigns approved volunteers to shifts
3. Volunteer checks in via QR
4. Volunteer checks in via GPS
5. Volunteer checks out
6. Organizer reviews suspicious attendance
7. System calculates approved hours
8. Admin audits attendance disputes

## Business rules
1. Only approved applicants can be assigned to event shifts unless admin override exists.
2. Attendance can be created only for valid event participants.
3. Check-in is allowed only inside configured time window.
4. GPS check-in must satisfy configured distance threshold from event or shift location.
5. QR check-in must validate token integrity and expiry.
6. Approved hours should be based on validated attendance duration and policy.
7. Missing check-out may place record into NeedsReview.
8. Organizer can manually correct attendance, but changes must be audited.
9. Attendance approval is a prerequisite for certificate generation and volunteer hour accumulation.

## Validation rules
- Event and volunteer identity are required
- Check-out cannot occur before check-in
- ApprovedHours cannot be negative
- GPS coordinates required for GPS method
- Duplicate active check-in should be prevented
- Shift capacity should be respected if enforced
- Manual override reason required if policy demands it

## Security and audit rules
- QR payload should not expose sensitive raw data
- GPS verification should store only necessary data
- All manual changes must be auditable
- Volunteer can see own attendance status only
- Organizer can manage attendance only for owned events
- Admin can inspect disputed records

## Dependencies
- Event Management for event and shift context
- Application Approval for approved participant eligibility
- Volunteer Profile for hour ledger integration
- Certificate and Badge for certificate eligibility
- Notification for reminders and attendance outcomes
- Geo service / QR service in Infrastructure

## Out of scope
- Biometric attendance unless explicitly requested
- Offline attendance sync unless explicitly requested

## Required outputs
- Shift and attendance entities
- QR/GPS verification abstractions
- Attendance service
- Shift assignment service
- Organizer operational controllers
- Volunteer self-service attendance status view
- Validators
- Unit tests and integration tests

## Suggested folder targets
- src/VolunteerHub.Application/Features/Attendance/
- src/VolunteerHub.Infrastructure/Services/QrCode/
- src/VolunteerHub.Infrastructure/Services/Geo/
- src/VolunteerHub.Web/Areas/Organizer/Controllers/AttendanceController.cs

## Suggested application services
- IAttendanceService
- IShiftAssignmentService
- IAttendanceVerificationService

## Suggested commands / operations
- CreateShift
- AssignVolunteerToShift
- CheckInByQr
- CheckInByGps
- CheckOut
- ApproveAttendance
- RejectAttendance
- MarkAbsent
- ManualAdjustAttendance
- GetAttendanceReport

## Integration notes
- On approved attendance, write to VolunteerHourLedger via controlled service.
- Certificate generation depends on this module’s approved attendance status.
- Notification may send reminders before shift start and results after review.

## Error scenarios
- Volunteer not approved for event
- Invalid or expired QR token
- GPS out of allowed range
- Duplicate check-in
- Check-out without check-in
- Organizer trying to edit non-owned event attendance
- Missing attendance review resolution

## Testing checklist
- Approved applicant can check in
- Non-approved applicant cannot check in
- GPS outside radius is rejected
- QR token expiry is enforced
- Approved hours calculated correctly
- Manual adjustments are audited
- Certificate eligibility depends on approved attendance