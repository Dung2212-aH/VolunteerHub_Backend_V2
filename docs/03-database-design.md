# Database Design

## Core tables
- Users
- Roles
- VolunteerProfiles
- OrganizerProfiles
- Events
- EventSkillRequirements
- EventApplications
- EventShifts
- Attendances
- Certificates
- Badges
- Sponsors
- Donations
- Ratings
- Notifications

## Important relationships
- One OrganizerProfile creates many Events
- One VolunteerProfile can apply to many Events
- One Event has many Applications
- One Event has many Shifts
- One Attendance belongs to one Event and one Volunteer