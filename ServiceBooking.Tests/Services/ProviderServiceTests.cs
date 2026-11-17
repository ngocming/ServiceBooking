using Xunit;
using ServiceBooking.Api.Data;
using ServiceBooking.Api.Models;
using ServiceBooking.Api.Services.Implementations;
using ServiceBooking.Tests.TestHelpers;
using ProviderModel = ServiceBooking.Api.Models.Provider;
using ProviderServiceModel = ServiceBooking.Api.Models.ProviderService;
using ProviderServiceImpl = ServiceBooking.Api.Services.Implementations.ProviderService;

namespace ServiceBooking.Tests.Services;

public class ProviderServiceTests
{
    private AppDbContext SetupBaseData(string dbName)
    {
        return DbContextHelper.CreateInMemoryDbContext(dbName);
    }

    [Fact]
    public async Task GetPublicProfileAsync_ExistingProvider_ReturnsProfileWithServicesAndSchedule()
    {
        var db = SetupBaseData(nameof(GetPublicProfileAsync_ExistingProvider_ReturnsProfileWithServicesAndSchedule));
        var user = new User { Id = 1, Username = "autocare_pro", Email = "pro@autocare.com", PasswordHash = "h", Role = "Provider" };
        var provider = new ProviderModel
        {
            Id = 10,
            UserId = 1,
            DisplayName = "AutoCare Pro Garage",
            Description = "Chuyen sua chua oto cao cap",
            Address = "123 Le Loi, Da Nang",
            Phone = "0905123456",
            IsAvailable = true,
            User = user
        };

        var service1 = new ProviderServiceModel
        {
            Id = 101,
            ProviderId = 10,
            Name = "Phủ Ceramic",
            Description = "Phủ bóng 9H",
            Price = 2500000,
            DurationMinutes = 180,
            IsAvailable = true
        };

        var scheduleSlot = new ProviderAvailability
        {
            Id = 1,
            ProviderId = 10,
            DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeSpan(8, 0, 0),
            EndTime = new TimeSpan(18, 0, 0),
            IsAvailable = true
        };

        db.Users.Add(user);
        db.Providers.Add(provider);
        db.ProviderServices.Add(service1);
        db.ProviderAvailabilities.Add(scheduleSlot);
        await db.SaveChangesAsync();

        var service = new ProviderServiceImpl(db);
        var profile = await service.GetPublicProfileAsync(10);

        Assert.NotNull(profile);
        Assert.Equal("AutoCare Pro Garage", profile.DisplayName);
        Assert.Single(profile.Services);
        Assert.Equal("Phủ Ceramic", profile.Services[0].Name);
        Assert.Single(profile.WeeklySchedule);
        Assert.Equal(DayOfWeek.Monday, profile.WeeklySchedule[0].DayOfWeek);
    }

    [Fact]
    public async Task GetPublicProfileAsync_NonExistentProvider_ReturnsNull()
    {
        var db = SetupBaseData(nameof(GetPublicProfileAsync_NonExistentProvider_ReturnsNull));
        var service = new ProviderServiceImpl(db);

        var profile = await service.GetPublicProfileAsync(999);
        Assert.Null(profile);
    }

    [Fact]
    public async Task GetPublicProfileAsync_CountsCompletedBookingsCorrectly()
    {
        var db = SetupBaseData(nameof(GetPublicProfileAsync_CountsCompletedBookingsCorrectly));
        var user = new User { Id = 1, Username = "prov_count", Email = "c@test.com", PasswordHash = "h", Role = "Provider" };
        var provider = new ProviderModel { Id = 20, UserId = 1, DisplayName = "Count Gara", User = user };
        var cust = new Customer { Id = 5, UserId = 2 };

        db.Users.AddRange(user, new User { Id = 2, Username = "cust", Email = "cu@test.com", PasswordHash = "h", Role = "Customer" });
        db.Providers.Add(provider);
        db.Customers.Add(cust);

        // 2 Completed, 1 Pending, 1 Cancelled
        db.Bookings.Add(new Booking { Id = 1, ProviderId = 20, CustomerId = 5, ProviderServiceId = 1, Status = BookingStatus.Completed });
        db.Bookings.Add(new Booking { Id = 2, ProviderId = 20, CustomerId = 5, ProviderServiceId = 1, Status = BookingStatus.Completed });
        db.Bookings.Add(new Booking { Id = 3, ProviderId = 20, CustomerId = 5, ProviderServiceId = 1, Status = BookingStatus.Pending });
        db.Bookings.Add(new Booking { Id = 4, ProviderId = 20, CustomerId = 5, ProviderServiceId = 1, Status = BookingStatus.Cancelled });
        await db.SaveChangesAsync();

        var service = new ProviderServiceImpl(db);
        var profile = await service.GetPublicProfileAsync(20);

        Assert.NotNull(profile);
        Assert.Equal(2, profile.CompletedBookingsCount);
    }
}
