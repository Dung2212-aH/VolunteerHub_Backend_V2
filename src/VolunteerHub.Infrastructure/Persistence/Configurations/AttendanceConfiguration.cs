using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VolunteerHub.Domain.Entities;

namespace VolunteerHub.Infrastructure.Persistence.Configurations;

public class EventShiftConfiguration : IEntityTypeConfiguration<EventShift>
{
    public void Configure(EntityTypeBuilder<EventShift> builder)
    {
        builder.HasKey(s => s.Id);
        
        builder.HasOne(s => s.Event)
            .WithMany(e => e.Shifts)
            .HasForeignKey(s => s.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(s => s.Title).IsRequired().HasMaxLength(200);
        
        builder.HasMany(s => s.Assignments)
            .WithOne(a => a.EventShift)
            .HasForeignKey(a => a.EventShiftId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(s => s.AttendanceRecords)
            .WithOne(a => a.EventShift)
            .HasForeignKey(a => a.EventShiftId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ShiftAssignmentConfiguration : IEntityTypeConfiguration<ShiftAssignment>
{
    public void Configure(EntityTypeBuilder<ShiftAssignment> builder)
    {
        builder.HasKey(a => a.Id);
        
        builder.HasIndex(a => new { a.EventShiftId, a.VolunteerProfileId }).IsUnique();

        builder.HasOne(a => a.VolunteerProfile)
            .WithMany()
            .HasForeignKey(a => a.VolunteerProfileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class AttendanceRecordConfiguration : IEntityTypeConfiguration<AttendanceRecord>
{
    public void Configure(EntityTypeBuilder<AttendanceRecord> builder)
    {
        builder.HasKey(a => a.Id);

        builder.HasIndex(a => a.EventId);
        builder.HasIndex(a => a.VolunteerProfileId);
        builder.HasIndex(a => a.Status);
        
        builder.HasIndex(a => new { a.EventShiftId, a.VolunteerProfileId }).IsUnique();

        builder.HasOne(a => a.Event)
            .WithMany()
            .HasForeignKey(a => a.EventId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.VolunteerProfile)
            .WithMany()
            .HasForeignKey(a => a.VolunteerProfileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
