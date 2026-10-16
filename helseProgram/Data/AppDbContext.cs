using helseProgram.Modeller;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace helseProgram.Data;

public class AppDbContext : DbContext
{
    public DbSet<PasientInfo> PasientInfo { get; set; }

    public DbSet<Medikamenter> Medikamenter { get; set; }

    public DbSet<MedikamentType> MedikamentType { get; set; }

    public DbSet<MedikamentVirkestoff> MedikamentVirkestoff { get; set; }

    public DbSet<MedikamentAdministrering> MedikamentAdministrering { get; set; }

    public DbSet<SimSesjon> SimSesjon { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=medicaljournal.db");
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PasientInfo>()
            .HasKey(p => p.PasientId);
        
        modelBuilder.Entity<Medikamenter>()
            .HasKey(m => m.MedikamentId);

        modelBuilder.Entity<MedikamentType>()
            .HasKey(mt => mt.MedikamentTypeId);

        modelBuilder.Entity<MedikamentVirkestoff>()
            .HasKey(mv => mv.MedikamentVirkestoffId);

        modelBuilder.Entity<MedikamentAdministrering>()
            .HasKey(m => m.MedikamentAdministreringId);

        modelBuilder.Entity<SimSesjon>()
            .HasKey(s => s.SimSesjonsId);

        modelBuilder.Entity<MedikamentType>()
            .HasOne(mt => mt.Medikamenter)
            .WithMany(m => m.MedikamentTyper)
            .HasForeignKey(mt => mt.MedikamentId);

        modelBuilder.Entity<MedikamentVirkestoff>()
            .HasOne(mv => mv.MedikamentType)
            .WithMany(mt => mt.Virkestoffer)
            .HasForeignKey(mv => mv.MedikamentTypeId);

        modelBuilder.Entity<MedikamentAdministrering>()
            .HasOne(m => m.Medikament)
            .WithMany()
            .HasForeignKey(m => m.MedikamentId);

        modelBuilder.Entity<MedikamentAdministrering>()
            .HasOne(m => m.Pasient)
            .WithMany()
            .HasForeignKey(m => m.PasientId);

        modelBuilder.Entity<MedikamentAdministrering>()
            .HasOne(m => m.SimSesjon)
            .WithMany()
            .HasForeignKey(m => m.SimSesjonsId);

    }
}