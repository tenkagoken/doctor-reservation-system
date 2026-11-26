using DoctorReservationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace DoctorReservationSystem.Data;

public class ReservationDbContext(DbContextOptions<ReservationDbContext> options) : DbContext(options)
{
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Doctor>().HasMany(d => d.Appointments).WithOne(a => a.Doctor)
            .HasForeignKey(a => a.DoctorId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Appointment>().HasOne(a => a.User).WithMany(u => u.Appointments)
            .HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Appointment>().HasIndex(a => new { a.DoctorId, a.StartsAt }).IsUnique();
        modelBuilder.Entity<Appointment>().ToTable(t => t.HasCheckConstraint("CK_Appointments_Time", "[EndsAt] > [StartsAt]"));
        modelBuilder.Entity<Appointment>().ToTable(t => t.HasCheckConstraint("CK_Appointments_Booking", "([IsBooked] = 0 AND [UserId] IS NULL) OR ([IsBooked] = 1 AND [UserId] IS NOT NULL)"));
        modelBuilder.Entity<User>().HasIndex(u => u.NationalId).IsUnique();
        modelBuilder.Entity<User>().Property(u => u.NationalId).IsUnicode(false).HasMaxLength(14);
    }
}
