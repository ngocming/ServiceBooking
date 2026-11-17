using Xunit;
using ServiceBooking.Api.Data;
using ServiceBooking.Api.DTOs.Booking;
using ServiceBooking.Api.Models;
using ServiceBooking.Api.Services.Implementations;
using ServiceBooking.Tests.TestHelpers;
using ProviderServiceModel = ServiceBooking.Api.Models.ProviderService;

namespace ServiceBooking.Tests.Services;

public class BookingServiceTests
{
    private (AppDbContext db, Customer customer, Provider provider, ProviderServiceModel service) SetupBaseData(string dbName)
    {
        var db = DbContextHelper.CreateInMemoryDbContext(dbName);

        var custUser = new User
        {
            Id = 1,
            Username = "customer_user",
            Email = "cust@test.com",
            PasswordHash = "hash",
            Role = "Customer"
        };
        var customer = new Customer { Id = 10, UserId = 1 };

        var provUser = new User
        {
            Id = 2,
            Username = "provider_user",
            Email = "prov@test.com",
            PasswordHash = "hash",
            Role = "Provider"
        };
        var provider = new Provider { Id = 20, UserId = 2, DisplayName = "Clean Masters", User = provUser };

        var providerService = new ProviderServiceModel
        {
            Id = 100,
            ProviderId = 20,
            Name = "Deep House Cleaning",
            Price = 120m,
            DurationMinutes = 60, // 1 hour duration
            IsAvailable = true,
            Provider = provider
        };

        db.Users.AddRange(custUser, provUser);
        db.Customers.Add(customer);
        db.Providers.Add(provider);
        db.ProviderServices.Add(providerService);
        db.SaveChanges();

        return (db, customer, provider, providerService);
    }

    [Fact]
    public async Task CreateAsync_ValidBooking_CreatesPendingBooking()
    {
        var (db, customer, provider, service) = SetupBaseData(nameof(CreateAsync_ValidBooking_CreatesPendingBooking));
        var bookingService = new BookingService(db);

        var bookingDate = DateTime.UtcNow.AddDays(1);
        var dto = new CreateBookingDto
        {
            ProviderServiceId = service.Id,
            BookingDate = bookingDate,
            Note = "Please bring cleaning supplies"
        };

        var result = await bookingService.CreateAsync(userId: 1, dto);

        Assert.NotNull(result);
        Assert.Equal(service.Id, result.ProviderServiceId);
        Assert.Equal("Pending", result.Status);
        Assert.Equal(service.Price, result.TotalPrice);
    }

    [Fact]
    public async Task CreateAsync_PastDate_ThrowsArgumentException()
    {
        var (db, customer, provider, service) = SetupBaseData(nameof(CreateAsync_PastDate_ThrowsArgumentException));
        var bookingService = new BookingService(db);

        var dto = new CreateBookingDto
        {
            ProviderServiceId = service.Id,
            BookingDate = DateTime.UtcNow.AddHours(-2),
            Note = "Past date"
        };

        await Assert.ThrowsAsync<ArgumentException>(() => bookingService.CreateAsync(userId: 1, dto));
    }

    [Fact]
    public async Task CreateAsync_OverlappingBooking_ThrowsInvalidOperationException()
    {
        var (db, customer, provider, service) = SetupBaseData(nameof(CreateAsync_OverlappingBooking_ThrowsInvalidOperationException));
        var bookingService = new BookingService(db);

        var baseTime = DateTime.UtcNow.AddDays(2).Date.AddHours(14); // 14:00 (duration: 60 mins -> ends at 15:00)

        // Existing active booking from 14:00 to 15:00
        db.Bookings.Add(new Booking
        {
            Id = 500,
            CustomerId = customer.Id,
            ProviderId = provider.Id,
            ProviderServiceId = service.Id,
            BookingDate = baseTime,
            Status = BookingStatus.Confirmed,
            TotalPrice = service.Price
        });
        await db.SaveChangesAsync();

        // Customer attempts to book at 14:30 (overlaps with 14:00 - 15:00)
        var overlappingDto = new CreateBookingDto
        {
            ProviderServiceId = service.Id,
            BookingDate = baseTime.AddMinutes(30),
            Note = "Overlap booking"
        };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => bookingService.CreateAsync(userId: 1, overlappingDto));
        Assert.Contains("already booked", ex.Message);
    }

    [Fact]
    public async Task CreateAsync_AfterExistingBooking_Succeeds()
    {
        var (db, customer, provider, service) = SetupBaseData(nameof(CreateAsync_AfterExistingBooking_Succeeds));
        var bookingService = new BookingService(db);

        var baseTime = DateTime.UtcNow.AddDays(2).Date.AddHours(14); // 14:00 to 15:00

        db.Bookings.Add(new Booking
        {
            Id = 501,
            CustomerId = customer.Id,
            ProviderId = provider.Id,
            ProviderServiceId = service.Id,
            BookingDate = baseTime,
            Status = BookingStatus.Confirmed,
            TotalPrice = service.Price
        });
        await db.SaveChangesAsync();

        // Customer books at 15:00 (right after previous service ends)
        var nextDto = new CreateBookingDto
        {
            ProviderServiceId = service.Id,
            BookingDate = baseTime.AddMinutes(60),
            Note = "Next slot"
        };

        var result = await bookingService.CreateAsync(userId: 1, nextDto);
        Assert.NotNull(result);
        Assert.Equal("Pending", result.Status);
    }

    [Fact]
    public async Task CancelAsync_WhenPendingOrConfirmed_Succeeds()
    {
        var (db, customer, provider, service) = SetupBaseData(nameof(CancelAsync_WhenPendingOrConfirmed_Succeeds));
        var bookingService = new BookingService(db);

        var bookingPending = new Booking
        {
            Id = 1,
            CustomerId = customer.Id,
            ProviderId = provider.Id,
            ProviderServiceId = service.Id,
            BookingDate = DateTime.UtcNow.AddDays(1),
            Status = BookingStatus.Pending,
            Customer = customer
        };
        var bookingConfirmed = new Booking
        {
            Id = 2,
            CustomerId = customer.Id,
            ProviderId = provider.Id,
            ProviderServiceId = service.Id,
            BookingDate = DateTime.UtcNow.AddDays(2),
            Status = BookingStatus.Confirmed,
            Customer = customer
        };
        db.Bookings.AddRange(bookingPending, bookingConfirmed);
        await db.SaveChangesAsync();

        // Customer cancels Pending
        await bookingService.CancelAsync(userId: 1, bookingId: 1);
        Assert.Equal(BookingStatus.Cancelled, (await db.Bookings.FindAsync(1))!.Status);

        // Customer cancels Confirmed
        await bookingService.CancelAsync(userId: 1, bookingId: 2);
        Assert.Equal(BookingStatus.Cancelled, (await db.Bookings.FindAsync(2))!.Status);
    }

    [Fact]
    public async Task CancelAsync_WhenCompleted_ThrowsInvalidOperationException()
    {
        var (db, customer, provider, service) = SetupBaseData(nameof(CancelAsync_WhenCompleted_ThrowsInvalidOperationException));
        var bookingService = new BookingService(db);

        var bookingCompleted = new Booking
        {
            Id = 3,
            CustomerId = customer.Id,
            ProviderId = provider.Id,
            ProviderServiceId = service.Id,
            BookingDate = DateTime.UtcNow.AddDays(-1),
            Status = BookingStatus.Completed,
            Customer = customer
        };
        db.Bookings.Add(bookingCompleted);
        await db.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(() => bookingService.CancelAsync(userId: 1, bookingId: 3));
    }

    [Fact]
    public async Task StatusTransitions_ProviderConfirmAndComplete_FollowsCorrectFlow()
    {
        var (db, customer, provider, service) = SetupBaseData(nameof(StatusTransitions_ProviderConfirmAndComplete_FollowsCorrectFlow));
        var bookingService = new BookingService(db);

        var booking = new Booking
        {
            Id = 10,
            CustomerId = customer.Id,
            ProviderId = provider.Id,
            ProviderServiceId = service.Id,
            BookingDate = DateTime.UtcNow.AddDays(1),
            Status = BookingStatus.Pending,
            Customer = customer
        };
        db.Bookings.Add(booking);
        await db.SaveChangesAsync();

        // 1. Provider confirms pending booking
        await bookingService.ConfirmAsync(userId: 2, bookingId: 10);
        var updated = await db.Bookings.FindAsync(10);
        Assert.Equal(BookingStatus.Confirmed, updated!.Status);

        // 2. Provider completes confirmed booking
        await bookingService.CompleteAsync(userId: 2, bookingId: 10);
        updated = await db.Bookings.FindAsync(10);
        Assert.Equal(BookingStatus.Completed, updated!.Status);
    }

    [Fact]
    public async Task CompleteAsync_WhenStillPending_ThrowsInvalidOperationException()
    {
        var (db, customer, provider, service) = SetupBaseData(nameof(CompleteAsync_WhenStillPending_ThrowsInvalidOperationException));
        var bookingService = new BookingService(db);

        var booking = new Booking
        {
            Id = 11,
            CustomerId = customer.Id,
            ProviderId = provider.Id,
            ProviderServiceId = service.Id,
            BookingDate = DateTime.UtcNow.AddDays(1),
            Status = BookingStatus.Pending,
            Customer = customer
        };
        db.Bookings.Add(booking);
        await db.SaveChangesAsync();

        // Provider attempts to Complete without Confirming first
        await Assert.ThrowsAsync<InvalidOperationException>(() => bookingService.CompleteAsync(userId: 2, bookingId: 11));
    }

    [Fact]
    public async Task ConfirmAsync_ByCustomer_ThrowsUnauthorizedAccessException()
    {
        var (db, customer, provider, service) = SetupBaseData(nameof(ConfirmAsync_ByCustomer_ThrowsUnauthorizedAccessException));
        var bookingService = new BookingService(db);

        var booking = new Booking
        {
            Id = 12,
            CustomerId = customer.Id,
            ProviderId = provider.Id,
            ProviderServiceId = service.Id,
            BookingDate = DateTime.UtcNow.AddDays(1),
            Status = BookingStatus.Pending,
            Customer = customer
        };
        db.Bookings.Add(booking);
        await db.SaveChangesAsync();

        // Customer (userId: 1) attempts to Confirm booking
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => bookingService.ConfirmAsync(userId: 1, bookingId: 12));
    }
}
