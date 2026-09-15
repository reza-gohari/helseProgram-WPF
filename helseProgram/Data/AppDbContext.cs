using Microsoft.EntityFrameworkCore;
using helseProgram.Modeller;

namespace helseProgram.Data;

public class AppDbContext : DbContext
{
    public DbSet<PasientInfo> PasientInfo { get; set; }

    public DbSet<Medikamenter> Medikamenter { get; set; }

    public DbSet<MedikamentAdministrering> MedikamentAdministrering { get; set; }

    public DbSet<SimSesjon> SimSesjon { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=medicaljournal.db");
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Medikamenter>()
            .HasKey(m => m.MedikamentId);

        modelBuilder.Entity<MedikamentAdministrering>()
            .HasKey(m => m.MedikamentAdministreringId);

        modelBuilder.Entity<PasientInfo>()
            .HasKey(p => p.PasientId);

        modelBuilder.Entity<SimSesjon>()
            .HasKey(s => s.SimSesjonsId);
    }
}