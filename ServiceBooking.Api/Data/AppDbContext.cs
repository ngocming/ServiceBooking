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
    public DbSet<ProviderService> ProviderServices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Provider)
            .WithOne(p => p.User)
            .HasForeignKey<Provider>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProviderService>()
            .HasOne(s => s.Provider)
            .WithMany(p => p.ProviderService)
            .HasForeignKey(s => s.ProviderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProviderService>()
        .ToTable("ProviderServices");
            
    }
}