using Microsoft.EntityFrameworkCore;
using ServiceBooking.Api.Models;
namespace ServiceBooking.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }
    public DbSet<User> Users { get; set; }
    public DbSet<Provider> Providers { get; set; }
    public DbSet<Service> Services { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Provider)
            .WithOne(p => p.User)
            .HasForeignKey<Provider>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Service>()
            .HasOne(s => s.Provider)
            .WithMany(p => p.Services)
            .HasForeignKey(s => s.ProviderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Service>()
            .Property(s => s.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Service>()
            .Property(s => s.Name)
            .HasMaxLength(100); 

        modelBuilder.Entity<Service>()
            .Property(s => s.Description)
            .HasMaxLength(500); 
            
    }
}