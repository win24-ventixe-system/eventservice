using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
namespace Data.Contexts;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<EventEntity> Events { get; set; }

    public DbSet<PackageEntity> Packages { get; set; }

    public DbSet<EventPackageEntity> EventsPackages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<EventPackageEntity>()
            .HasKey(ep => ep.Id); // You're using a single Id

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
