# Module: Event Management

## Purpose
Allow organizers to create, update, publish, and manage volunteer events.

## Responsibilities
- Create event
- Update event
- Publish or unpublish event
- Search events
- View event details
- Manage skill requirements
- Manage capacity and location

## Related actors
- Organizer
- Volunteer
- Admin

## Entities
- Event
- EventSkillRequirement
- EventLocation
- EventShift

## Key fields
Event:
- Id
- OrganizerId
- Title
- Description
- StartAt
- EndAt
- Address
- Latitude
- Longitude
- Capacity
- Status

## Business rules
- Only verified organizers can publish events
- Capacity must be greater than zero
- EndAt must be later than StartAt
- Completed events cannot be edited by organizer
- Admin can moderate events

## Dependencies
- Organizer module
- Notification module

## Required outputs
- Domain entities
- EF Core configurations
- Repository layer
- Service layer
- DTOs
- Validators
- MVC controllers
- Unit tests