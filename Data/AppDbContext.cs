using HospitalSanVicente.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalSanVicente.Data;

public class AppDbContext : DbContext
{
    public DbSet<Patient>  Patients { get; set; }
    public DbSet<Doctor>  Doctors { get; set; }
    public DbSet<Appointment>  Appointments { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Doctor>()
            .HasMany(f => f.Appoinments)
            .WithOne(p => p.doctor)
            .HasForeignKey(p => p.DoctorId);
        
        modelBuilder.Entity<Appointment>()
            .HasOne(p => p.patient)
            .WithMany(r => r.Appoinments)
            .HasForeignKey(p => p.PatientId);
    }
    
    
}