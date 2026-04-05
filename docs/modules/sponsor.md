# Module: Sponsor

## Purpose
Manage sponsor identity, sponsorship pledges and donations, event-linked contributions, and sponsor visibility into campaign outcomes.

## Main actors
- Sponsor
- Organizer
- Admin

## Responsibilities
- Create and manage sponsor profile
- Record monetary donations
- Record in-kind donations
- Link contributions to events or campaigns
- Track donation status
- Provide sponsor history and reporting
- Allow organizer/admin to acknowledge received support where applicable

## Scope
This module handles sponsor-side engagement and contribution records.
It does not perform external payment gateway integration unless explicitly added.

## Related modules
- Auth
- Event Management
- Organizer
- Notification
- Admin

## Core entities
- SponsorProfile
- SponsorshipCampaign (optional)
- Donation
- DonationItem
- DonationAllocation
- DonationReceipt
- SponsorshipAcknowledgement

## Suggested aggregate roots
- SponsorProfile
- Donation

## Important fields

### SponsorProfile
- Id
- UserId
- DisplayName
- OrganizationName
- SponsorType
- ContactEmail
- ContactPhone
- WebsiteUrl
- Description
- LogoUrl
- IsPublic
- CreatedAt
- UpdatedAt

### Donation
- Id
- SponsorProfileId
- EventId
- CampaignId
- DonationType
- MonetaryAmount
- Currency
- Status
- Note
- PledgedAt
- ConfirmedAt
- ReceivedAt
- ReceiptNumber

### DonationItem
- Id
- DonationId
- ItemName
- Quantity
- Unit
- EstimatedValue
- Notes

### DonationAllocation
- Id
- DonationId
- EventId
- AllocationNote
- AllocatedAt

### SponsorshipAcknowledgement
- Id
- DonationId
- AcknowledgedByUserId
- Message
- AcknowledgedAt

## Donation types
- Monetary
- InKind
- Mixed

## Donation statuses
- Pledged
- PendingConfirmation
- Confirmed
- Received
- Cancelled
- Rejected

## Main use cases
1. Sponsor creates profile
2. Sponsor pledges donation to event or campaign
3. Sponsor submits in-kind donation details
4. Organizer/admin confirms received donation
5. Sponsor views donation history and impact context
6. System issues acknowledgement or receipt info if supported

## Business rules
1. Every donation must belong to a sponsor profile.
2. Donation may optionally link to a specific event or broader campaign based on system design.
3. Monetary and in-kind donations must be represented clearly and auditable.
4. Organizer cannot mark donation as received for unrelated events.
5. Cancelled or rejected donations should not count in impact summaries.
6. Admin can correct donation records in exceptional cases.
7. Donation acknowledgements should preserve audit history.
8. Public sponsor visibility depends on sponsor profile setting and platform policy.

## Validation rules
- Sponsor identity required
- Donation must specify type
- Monetary donation requires amount > 0
- In-kind donation requires at least one donation item
- Currency required for monetary donation
- Event reference must exist if provided
- Status transitions must be valid
- Receipt number uniqueness if receipts are generated internally

## Security and privacy rules
- Sponsor can view only own donation history
- Organizer can view donations linked to owned events only
- Admin can view all
- Sensitive billing/contact details must be protected
- Manual status changes must be audited

## Dependencies
- Auth for sponsor identity
- Event Management for target event lookup
- Organizer for event ownership and acknowledgement
- Notification for sponsor updates
- Admin for oversight

## Out of scope
- External payment gateway implementation unless explicitly requested
- Tax/legal invoicing workflow unless explicitly designed

## Required outputs
- Sponsor and donation entities
- Repository and service layer
- Sponsor self-service controllers
- Organizer/admin acknowledgement flows
- Donation history queries
- Validators
- Unit tests and integration tests

## Suggested folder targets
- src/VolunteerHub.Application/Features/Sponsors/
- src/VolunteerHub.Web/Areas/Sponsor/Controllers/
- src/VolunteerHub.Web/Areas/Organizer/Controllers/SponsorSupportController.cs

## Suggested application services
- ISponsorProfileService
- IDonationService
- IDonationAcknowledgementService

## Suggested commands / operations
- CreateSponsorProfile
- UpdateSponsorProfile
- CreateDonation
- UpdateDonation
- CancelDonation
- ConfirmDonation
- MarkDonationReceived
- GetMyDonations
- GetEventDonationsSummary

## Integration notes
- Event pages may display sponsor support summaries if policy permits.
- Admin reporting may consume donation status aggregates.
- Notification should inform sponsor of status changes and acknowledgements.

## Error scenarios
- Sponsor profile missing
- Invalid donation amount
- In-kind donation without items
- Organizer accessing unrelated donation
- Invalid status transition
- Duplicate receipt number
- Unauthorized donation history access

## Testing checklist
- Sponsor can create donation
- In-kind donation requires items
- Organizer can acknowledge only owned event donation
- Cancelled donations excluded from totals
- Sponsor sees only own history
- Admin can review all records