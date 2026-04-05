# Architecture

## Solution layout
- VolunteerHub.Web
- VolunteerHub.Application
- VolunteerHub.Domain
- VolunteerHub.Infrastructure
- VolunteerHub.Contracts

## Design rules
- Controllers must stay thin
- No direct DbContext usage in Controllers
- Application services handle business use cases
- Infrastructure implements persistence and external integrations
- Domain must not depend on Infrastructure
- Use Areas for Admin, Organizer, Volunteer sections in MVC
- Use async for all I/O operations