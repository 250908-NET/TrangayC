using Microsoft.EntityFrameworkCore;
using CareLink.WebApi.Models;

namespace CareLink.WebApi.Data;

public class CareLinkDbContext(DbContextOptions<CareLinkDbContext> options) : DbContext(options)
{
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<DoctorPatient> DoctorPatients => Set<DoctorPatient>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DoctorPatient>()
            .HasKey(dp => new { dp.DoctorId, dp.PatientId });

        modelBuilder.Entity<DoctorPatient>()
            .HasOne(dp => dp.Doctor)
            .WithMany(d => d.DoctorPatients)
            .HasForeignKey(dp => dp.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DoctorPatient>()
            .HasOne(dp => dp.Patient)
            .WithMany(p => p.DoctorPatients)
            .HasForeignKey(dp => dp.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }
}
