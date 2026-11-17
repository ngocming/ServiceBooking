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
    public DbSet<ServiceCategory> ServiceCategories { get; set; }

    public DbSet<Booking> Bookings { get; set; }
    public DbSet<BookingStatusHistory> BookingStatusHistories { get; set; }
    public DbSet<ProviderAvailability> ProviderAvailabilities { get; set; }
    public DbSet<AvailabilityException> AvailabilityExceptions { get; set; }

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

        modelBuilder.Entity<ProviderService>()
            .HasOne(s => s.Category)
            .WithMany(c => c.Services)
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<ServiceCategory>()
            .HasIndex(c => c.Slug)
            .IsUnique();

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

        modelBuilder.Entity<BookingStatusHistory>()
            .HasOne(h => h.Booking)
            .WithMany(b => b.StatusHistories)
            .HasForeignKey(h => h.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BookingStatusHistory>()
            .HasOne(h => h.ChangedByUser)
            .WithMany()
            .HasForeignKey(h => h.ChangedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ProviderAvailability>()
            .HasOne(pa => pa.Provider)
            .WithMany(p => p.Availabilities)
            .HasForeignKey(pa => pa.ProviderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AvailabilityException>()
            .HasOne(ae => ae.Provider)
            .WithMany(p => p.AvailabilityExceptions)
            .HasForeignKey(ae => ae.ProviderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}