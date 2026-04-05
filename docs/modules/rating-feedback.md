# Module: Rating and Feedback

## Purpose
Capture two-way trust signals between volunteers and organizers, support feedback after participation, and manage disputes or complaints related to ratings.

## Main actors
- Volunteer
- Organizer
- Admin

## Responsibilities
- Allow volunteers to rate organizers
- Allow organizers to rate volunteers
- Store textual feedback
- Enforce eligibility for rating
- Prevent duplicate abusive rating submissions
- Handle complaints/disputes
- Provide trust summaries and averages
- Support moderation of inappropriate feedback

## Scope
This module manages post-participation ratings and complaints.
It does not verify attendance directly, but depends on completed participation.

## Related modules
- Attendance
- Event Management
- Volunteer Profile
- Organizer
- Admin
- Notification

## Core entities
- Rating
- RatingCriteriaScore (optional)
- FeedbackComment
- RatingComplaint
- RatingModerationAction
- TrustSummaryProjection (read model)

## Suggested aggregate roots
- Rating
- RatingComplaint

## Important fields

### Rating
- Id
- EventId
- RaterUserId
- RateeType
- RateeReferenceId
- Score
- Comment
- Direction
- Status
- SubmittedAt

### RatingCriteriaScore
- Id
- RatingId
- CriteriaCode
- Score

### RatingComplaint
- Id
- RatingId
- ComplainantUserId
- Reason
- Status
- SubmittedAt
- ResolvedAt
- ResolvedByUserId
- ResolutionNote

### RatingModerationAction
- Id
- RatingId
- ActionType
- PreviousStatus
- NewStatus
- ActionByUserId
- ActionAt
- Note

## Directions
- VolunteerToOrganizer
- OrganizerToVolunteer

## Rating statuses
- Active
- Hidden
- UnderReview
- Removed

## Main use cases
1. Volunteer rates organizer after completed participation
2. Organizer rates volunteer after completed participation
3. User views trust summary
4. User submits complaint about abusive or unfair rating
5. Admin moderates rating and complaint
6. System updates aggregate trust score

## Business rules
1. Ratings are allowed only after completed eligible participation.
2. Each rating direction should be limited to one active rating per event-participant relationship unless update policy is defined.
3. Users cannot rate themselves.
4. Organizer can rate only volunteers who actually participated in the organizer’s event.
5. Volunteer can rate only organizer of attended event.
6. Hidden or removed ratings must not count in public averages.
7. Complaints must be auditable.
8. Admin moderation can hide or remove ratings for policy violations.

## Validation rules
- Score must be within allowed range, e.g. 1 to 5
- Comment length must respect limits
- Eligible participation required
- Duplicate active rating must be prevented
- Complaint reason is required
- Ratee reference must match event participation context

## Security and moderation rules
- Users can view own submitted ratings and ratings received according to visibility rules
- Complaint handling is admin-only beyond submission
- Private moderation notes are not public
- Anti-abuse rate limiting may be added if needed

## Dependencies
- Attendance for completed participation validation
- Organizer and Volunteer Profile for trust summaries
- Admin for moderation
- Notification for complaint or rating submission notices if needed

## Out of scope
- AI sentiment analysis unless explicitly added
- Public anonymous review forums unless explicitly added

## Required outputs
- Rating and complaint entities
- Eligibility service
- Trust summary query service
- Moderation service
- Volunteer and organizer submission controllers
- Admin moderation controller
- Validators
- Unit tests and integration tests

## Suggested folder targets
- src/VolunteerHub.Application/Features/Ratings/
- src/VolunteerHub.Web/Areas/Volunteer/Controllers/FeedbackController.cs
- src/VolunteerHub.Web/Areas/Organizer/Controllers/FeedbackController.cs
- src/VolunteerHub.Web/Areas/Admin/Controllers/RatingModerationController.cs

## Suggested application services
- IRatingService
- IRatingEligibilityService
- IComplaintService
- ITrustSummaryService

## Suggested commands / operations
- SubmitVolunteerToOrganizerRating
- SubmitOrganizerToVolunteerRating
- UpdateRating
- SubmitRatingComplaint
- ModerateRating
- GetTrustSummary

## Integration notes
- Trust summaries may be exposed on organizer public profiles and volunteer review summaries.
- Only completed validated attendance should unlock rating.
- Admin moderation should recalculate aggregate trust summaries when rating visibility changes.

## Error scenarios
- Ineligible participation
- Duplicate rating
- Self-rating attempt
- Complaint on nonexistent rating
- Non-admin moderation attempt
- Removed rating included in public score by mistake

## Testing checklist
- Eligible participant can submit rating
- Ineligible user cannot submit rating
- Duplicate rating blocked
- Complaint submission works
- Hidden rating excluded from average
- Organizer cannot rate volunteer from unrelated event