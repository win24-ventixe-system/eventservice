using Data.Entities;
using Microsoft.EntityFrameworkCore;
namespace Data.Contexts;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<EventEntity> Events { get; set; }

    public DbSet<PackageEntity> Packages { get; set; }

    public DbSet<EventPackageEntity> EventsPackages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure PackageEntity ID for DB-generated GUIDs
        modelBuilder.Entity<PackageEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                  .ValueGeneratedOnAdd()         
                  .HasDefaultValueSql("NEWID()"); 
            entity.Property(e => e.Id).HasMaxLength(36);
        });

        
        modelBuilder.Entity<EventEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                  .ValueGeneratedOnAdd()
                  .HasDefaultValueSql("NEWID()");
            entity.Property(e => e.Id).HasMaxLength(36);
        });
        modelBuilder.Entity<EventPackageEntity>(entity =>
        {
            entity.HasKey(ep => ep.Id);
            entity.Property(ep => ep.Id)
                  .ValueGeneratedOnAdd()
                  .HasDefaultValueSql("NEWID()");
            entity.Property(ep => ep.Id).HasMaxLength(36); 
                                                           
        });


        modelBuilder.Entity<EventPackageEntity>()
            .HasKey(ep => ep.Id);
       
        modelBuilder.Entity<EventPackageEntity>()
            .HasOne(ep => ep.Event)
            .WithMany(e => e.EventsPackages)
            .HasForeignKey(ep => ep.EventId);

        modelBuilder.Entity<EventPackageEntity>()
            .HasOne(ep => ep.Package)
            .WithMany(p => p.EventsPackages)
            .HasForeignKey(ep => ep.PackageId);
    }
}
