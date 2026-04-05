# Volunteer Hub - Project Overview

## Background
Volunteer Hub solves fragmented volunteer coordination by providing a transparent platform connecting volunteers, organizers, sponsors, and admins.

## Main goals
- Match volunteers with suitable events
- Improve organizer trust verification
- Digitally track contributions, hours, certificates, and badges

## Actors
- Volunteer
- Organizer
- Sponsor
- Admin

## Main modules
- Auth
- Volunteer Profile
- Organizer
- Event Management
- Application Approval
- Attendance
- Certificate and Badge
- Sponsor
- Rating and Feedback
- Notification
- Admin

## Tech decisions
- ASP.NET Core MVC
- My SQL
- EF Core
- Identity
- Areas for role-based sections
- Layered architecture

## Constraints
- Keep solution maintainable
- Avoid business logic in controllers
- Support future mobile or API expansion