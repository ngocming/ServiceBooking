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
    public DbSet<Customer> Customers { get; set; }
    public DbSet<ProviderService> ProviderServices { get; set; }

    public DbSet<Booking> Bookings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Provider)
            .WithOne(p => p.User)
            .HasForeignKey<Provider>(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<ProviderService>()
            .HasOne(s => s.Provider)
            .WithMany(p => p.ProviderServices)
            .HasForeignKey(s => s.ProviderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ProviderService>()
            .ToTable("ProviderServices")
            .Property(ps => ps.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Customer>()
            .HasOne(c => c.User)
            .WithOne(u => u.Customer)
            .HasForeignKey<Customer>(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Customer>()
            .HasIndex(c => c.UserId)
            .IsUnique();

        modelBuilder.Entity<Provider>()
            .HasIndex(p => p.UserId)
            .IsUnique();
            
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Customer)
            .WithMany()
            .HasForeignKey(b => b.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.ProviderService)
            .WithMany()
            .HasForeignKey(b => b.ProviderServiceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}