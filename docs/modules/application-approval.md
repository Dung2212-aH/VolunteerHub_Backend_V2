# Module: Application Approval

## Purpose
Handle volunteer applications to events, organizer review decisions, interview workflow if applicable, and final approval or rejection.

## Main actors
- Volunteer
- Organizer
- Admin

## Responsibilities
- Submit application to event
- Withdraw application if allowed
- Review applicants
- Approve/reject/waitlist applicants
- Store interview notes or screening decisions
- Prevent duplicate applications
- Provide organizer applicant dashboard
- Provide volunteer application status tracking

## Scope
This module covers the lifecycle from application submission until final decision before participation.
It does not perform attendance check-in or issue certificates.

## Related modules
- Volunteer Profile
- Event Management
- Organizer
- Attendance
- Notification

## Core entities
- EventApplication
- ApplicationReviewNote
- InterviewSchedule (optional)
- ApplicationDecisionHistory

## Suggested aggregate roots
- EventApplication

## Important fields

### EventApplication
- Id
- EventId
- VolunteerProfileId
- Status
- AppliedAt
- MotivationText
- AvailabilityNote
- ReviewedAt
- ReviewedBy
- RejectionReason
- IsWithdrawn
- WithdrawnAt

### ApplicationReviewNote
- Id
- EventApplicationId
- AuthorUserId
- NoteType
- Content
- CreatedAt

### InterviewSchedule
- Id
- EventApplicationId
- StartsAt
- EndsAt
- MeetingUrl
- Status
- CreatedAt

### ApplicationDecisionHistory
- Id
- EventApplicationId
- PreviousStatus
- NewStatus
- ChangedByUserId
- ChangedAt
- Reason

## Statuses
- Pending
- UnderReview
- InterviewScheduled
- Approved
- Rejected
- Waitlisted
- Withdrawn
- Cancelled

## Main use cases
1. Volunteer applies to event
2. Volunteer checks application status
3. Volunteer withdraws application before cutoff
4. Organizer reviews applicants
5. Organizer shortlists or schedules interview
6. Organizer approves or rejects application
7. System notifies volunteer of decision
8. Admin audits disputes or overrides abnormal cases

## Business rules
1. Volunteer can apply only once to the same event unless previous application was cancelled under explicitly allowed policy.
2. Volunteer cannot apply after event registration deadline or when event is closed.
3. Organizer can review only applications for events owned by their organization.
4. Approval must not exceed event capacity unless waitlist policy explicitly allows overflow handling.
5. Withdrawn applications should not be treated as active participants.
6. Interview notes are private to organizer/admin review scope.
7. Approved application is a prerequisite for attendance participation.
8. Admin can override application status in exceptional moderation scenarios.

## Validation rules
- Motivation text length should respect limits if required
- EventId and VolunteerProfileId are required
- Duplicate active application is not allowed
- Withdrawal allowed only in configured states
- Organizer decision requires valid current state
- Approval must respect capacity constraints
- Rejection reason required if policy says so

## Security and visibility rules
- Volunteer can see only own applications
- Organizer can see only applications for owned events
- Private review notes must not be visible to applicants
- Admin can audit full history
- Status changes must be auditable

## Dependencies
- Volunteer Profile for applicant summary
- Event Management for event state/capacity/deadline
- Organizer for ownership and permissions
- Notification for decision messages
- Attendance for approved participant list

## Out of scope
- Full external interview meeting integration unless later added
- AI matching/scoring engine unless explicitly requested

## Required outputs
- Application entities
- Status enums
- Repository and service layer
- Applicant summary query model
- Volunteer and organizer controllers
- Validation rules
- History tracking
- Unit tests and integration tests

## Suggested folder targets
- src/VolunteerHub.Application/Features/Applications/
- src/VolunteerHub.Web/Areas/Volunteer/Controllers/MyApplicationsController.cs
- src/VolunteerHub.Web/Areas/Organizer/Controllers/EventApplicantsController.cs

## Suggested application services
- IEventApplicationService
- IApplicationReviewService
- IApplicantQueryService

## Suggested commands / operations
- ApplyToEvent
- WithdrawApplication
- GetMyApplications
- GetEventApplicants
- ApproveApplication
- RejectApplication
- WaitlistApplication
- ScheduleInterview
- AddReviewNote

## Integration notes
- Attendance participant lists should be derived from approved applications.
- Event capacity checks must coordinate with Event Management.
- Notification must be triggered on application submission and decision changes.

## Error scenarios
- Event not open for registration
- Duplicate application
- Volunteer profile incomplete if policy requires completion
- Organizer reviewing non-owned event
- Capacity exceeded
- Invalid status transition
- Volunteer tries to read someone else’s application

## Testing checklist
- Volunteer can apply once only
- Closed event rejects new applications
- Organizer can approve only owned event applicants
- Approved count respects capacity
- Withdrawn application is removed from active pool
- Volunteer cannot see organizer private notes