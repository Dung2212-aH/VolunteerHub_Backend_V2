# Module: Admin

## Purpose
Provide platform governance, moderation, catalog management, oversight dashboards, dispute handling, and privileged administrative controls.

## Main actors
- Admin

## Responsibilities
- Review and verify organizers
- Manage skill catalog and other controlled master data
- Moderate users, events, ratings, complaints
- View system dashboard and impact reports
- Override critical workflow states in exceptional cases
- Audit operational and moderation history
- Manage platform policies/config flags if included

## Scope
This module provides administrative control across the platform.
It may orchestrate other modules but should avoid directly duplicating their business logic.

## Related modules
- Organizer
- Event Management
- Application Approval
- Attendance
- Certificate and Badge
- Rating and Feedback
- Notification
- Sponsor
- Volunteer Profile
- Auth

## Core entities
- AdminActionLog
- SkillCatalogItem
- ComplaintCase
- ModerationCase
- SystemSetting (optional)
- ImpactReportSnapshot (optional read model)

## Suggested aggregate roots
- SkillCatalogItem
- ComplaintCase
- ModerationCase

## Important fields

### AdminActionLog
- Id
- AdminUserId
- ActionType
- EntityType
- EntityId
- Description
- CreatedAt

### SkillCatalogItem
- Id
- Code
- Name
- Category
- IsActive
- SortOrder
- CreatedAt
- UpdatedAt

### ComplaintCase
- Id
- SourceType
- SourceReferenceId
- SubmittedByUserId
- Reason
- Status
- AssignedAdminUserId
- SubmittedAt
- ResolvedAt
- ResolutionNote

### ModerationCase
- Id
- TargetType
- TargetReferenceId
- Reason
- Status
- Decision
- CreatedAt
- DecidedAt
- DecidedByAdminUserId

## Main use cases
1. Admin approves or rejects organizer verification
2. Admin moderates event or profile issues
3. Admin handles complaints about ratings or participation
4. Admin manages skill catalog
5. Admin views operational and social impact dashboards
6. Admin suspends abusive users or entities where allowed
7. Admin audits logs and histories

## Business rules
1. Only users with Admin role can access this module.
2. All admin actions affecting user-visible status must be auditable.
3. Admin overrides should be exceptional and clearly logged.
4. Catalog changes should not silently corrupt historical data; snapshots or stable references should be used where needed.
5. Dashboard metrics must exclude invalid/revoked/cancelled records according to reporting rules.
6. Sensitive actions such as suspension, rejection, revocation, and moderation removal require reason capture.
7. Admin should not directly bypass security boundaries without audit logging.

## Validation rules
- Skill code must be unique
- Moderation decisions require reason when policy requires
- Complaint resolution requires status transition validity
- Admin action log entry required for critical changes
- Dashboard date ranges must be valid if queried

## Security and governance rules
- Admin area must be role-protected
- Every critical action should write AdminActionLog
- Private legal documents and sensitive user data must remain access-controlled
- Read and write permissions inside admin area may later be split into finer policies

## Dependencies
- Organizer for verification review
- Event Management for event moderation
- Rating and Feedback for complaints/moderation
- Auth for account suspension/role controls
- Volunteer Profile for skill catalog usage
- Sponsor for donation oversight
- Notification for outcome notices

## Out of scope
- External BI tools unless explicitly integrated
- Complex multi-tenant admin hierarchy unless separately designed

## Required outputs
- Admin entities
- Audit logging service
- Skill catalog management
- Complaint handling flows
- Dashboard query service
- Admin MVC area controllers
- Validators
- Unit tests and integration tests

## Suggested folder targets
- src/VolunteerHub.Application/Features/Admin/
- src/VolunteerHub.Web/Areas/Admin/Controllers/

## Suggested application services
- IAdminAuditService
- ISkillCatalogService
- IComplaintModerationService
- IImpactReportService
- IAdminGovernanceService

## Suggested commands / operations
- ApproveOrganizerVerification
- RejectOrganizerVerification
- SuspendUser
- ModerateEvent
- ResolveComplaint
- CreateSkill
- UpdateSkill
- DisableSkill
- GetImpactDashboard

## Dashboard metrics suggestions
- Total volunteers
- Total organizers
- Total published events
- Total completed events
- Total volunteer hours
- Total certificates issued
- Total badges awarded
- Total donations confirmed
- Average organizer trust score
- Average volunteer trust score

## Integration notes
- Admin module should call domain/application services of other modules rather than duplicating business logic.
- Audit logging should be cross-cutting and reusable.
- Reporting should use projections/read models where appropriate.

## Error scenarios
- Non-admin access
- Missing moderation target
- Invalid complaint status transition
- Duplicate skill code
- Dashboard query timeout or large aggregation issues
- Sensitive data exposed accidentally in admin grids

## Testing checklist
- Non-admin cannot access admin area
- Critical admin actions are logged
- Skill catalog unique code enforced
- Complaint resolution updates status properly
- Dashboard excludes cancelled/revoked records as defined