using Microsoft.EntityFrameworkCore;
using ServiceBooking.Api.Data;
using ServiceBooking.Api.DTOs.Booking;
using ServiceBooking.Api.Models;
using ServiceBooking.Api.Services.Interfaces;

namespace ServiceBooking.Api.Services.Implementations;

public class BookingService : IBookingService
{
    private readonly AppDbContext _dbContext;

    public BookingService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<BookingResponseDto> CreateAsync(
        int userId,
        CreateBookingDto dto)
    {
        var customer = await _dbContext.Customers
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (customer == null)
        {
            throw new KeyNotFoundException("Customer not found.");
        }

        if (dto.ProviderServiceId <= 0)
        {
            throw new ArgumentException("ProviderServiceId is required.");
        }

        if (dto.BookingDate <= DateTime.UtcNow)
        {
            throw new ArgumentException("Booking date must be in the future.");
        }

        var providerService = await _dbContext.ProviderServices
            .Include(ps => ps.Provider)
            .FirstOrDefaultAsync(ps => ps.Id == dto.ProviderServiceId);

        if (providerService == null)
        {
            throw new KeyNotFoundException("Provider service not found.");
        }

        if (!providerService.IsAvailable)
        {
            throw new InvalidOperationException("This service is currently unavailable for booking.");
        }

        var durationMinutes = providerService.DurationMinutes > 0 ? providerService.DurationMinutes : 60;
        var newStart = dto.BookingDate;
        var newEnd = newStart.AddMinutes(durationMinutes);

        // Fetch non-cancelled, non-rejected bookings for the same provider within a +/- 24h window
        var relevantBookings = await _dbContext.Bookings
            .Include(b => b.ProviderService)
            .Where(b => b.ProviderId == providerService.ProviderId &&
                        b.Status != BookingStatus.Cancelled &&
                        b.Status != BookingStatus.Rejected &&
                        b.BookingDate >= newStart.AddHours(-24) &&
                        b.BookingDate <= newEnd.AddHours(24))
            .ToListAsync();

        var hasOverlap = relevantBookings.Any(b =>
        {
            var existingDuration = b.ProviderService?.DurationMinutes > 0 ? b.ProviderService.DurationMinutes : 60;
            var existingStart = b.BookingDate;
            var existingEnd = existingStart.AddMinutes(existingDuration);

            // Two intervals [A_start, A_end] and [B_start, B_end] overlap if A_start < B_end and A_end > B_start
            return newStart < existingEnd && newEnd > existingStart;
        });

        if (hasOverlap)
        {
            throw new InvalidOperationException("This service or provider is already booked during this time interval.");
        }

        var booking = new Booking
        {
            CustomerId = customer.Id,
            ProviderId = providerService.ProviderId,
            ProviderServiceId = providerService.Id,
            BookingDate = dto.BookingDate,
            Note = dto.Note,
            Status = BookingStatus.Pending,
            TotalPrice = providerService.Price,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Bookings.Add(booking);
        await _dbContext.SaveChangesAsync();

        var savedBooking = await _dbContext.Bookings
            .Include(b => b.ProviderService)
            .Include(b => b.Provider).ThenInclude(p => p.User)
            .Include(b => b.Customer)
            .FirstAsync(b => b.Id == booking.Id);

        return MapToDto(savedBooking);
    }

    public async Task<IEnumerable<BookingResponseDto>> GetCustomerBookingsAsync(int userId)
    {
        var customer = await _dbContext.Customers
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (customer == null)
        {
            throw new KeyNotFoundException("Customer not found.");
        }

        var bookings = await _dbContext.Bookings
            .Where(b => b.CustomerId == customer.Id)
            .Include(b => b.ProviderService)
            .Include(b => b.Provider)
                .ThenInclude(p => p.User)
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync();

        return bookings.Select(MapToDto);
    }

    public async Task<IEnumerable<BookingResponseDto>> GetProviderBookingsAsync(int userId)
    {
        var provider = await _dbContext.Providers
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (provider == null)
        {
            throw new KeyNotFoundException("Provider not found.");
        }

        var bookings = await _dbContext.Bookings
            .Where(b => b.ProviderId == provider.Id)
            .Include(b => b.ProviderService)
            .Include(b => b.Customer)
                .ThenInclude(c => c.User)
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync();

        return bookings.Select(MapToDto);
    }

    public async Task<BookingResponseDto?> GetByIdAsync(int userId, int bookingId)
    {
        var booking = await _dbContext.Bookings
            .Include(b => b.ProviderService)
            .Include(b => b.Provider)
                .ThenInclude(p => p.User)
            .Include(b => b.Customer)
            .FirstOrDefaultAsync(b => b.Id == bookingId);

        if (booking == null)
        {
            return null;
        }

        var isCustomer = booking.Customer.UserId == userId;
        var isProvider = await _dbContext.Providers
            .AnyAsync(p => p.Id == booking.ProviderId && p.UserId == userId);

        if (!isCustomer && !isProvider)
        {
            throw new UnauthorizedAccessException("You are not allowed to view this booking.");
        }

        return MapToDto(booking);
    }

    public async Task CancelAsync(int userId, int bookingId)
    {
        var booking = await _dbContext.Bookings
            .Include(b => b.Customer)
            .FirstOrDefaultAsync(b => b.Id == bookingId);

        if (booking == null)
        {
            throw new KeyNotFoundException("Booking not found.");
        }

        var isCustomer = booking.Customer.UserId == userId;
        var isProvider = await _dbContext.Providers
            .AnyAsync(p => p.Id == booking.ProviderId && p.UserId == userId);

        if (!isCustomer && !isProvider)
        {
            throw new UnauthorizedAccessException("Only the customer or provider can cancel this booking.");
        }

        if (booking.Status != BookingStatus.Pending && booking.Status != BookingStatus.Confirmed)
        {
            throw new InvalidOperationException("Only pending or confirmed bookings can be cancelled.");
        }

        booking.Status = BookingStatus.Cancelled;
        await _dbContext.SaveChangesAsync();
    }

    public async Task ConfirmAsync(int userId, int bookingId)
    {
        var booking = await _dbContext.Bookings
            .FirstOrDefaultAsync(b => b.Id == bookingId);

        if (booking == null)
        {
            throw new KeyNotFoundException("Booking not found.");
        }

        var isProvider = await _dbContext.Providers
            .AnyAsync(p => p.Id == booking.ProviderId && p.UserId == userId);

        if (!isProvider)
        {
            throw new UnauthorizedAccessException("Only the provider can confirm this booking.");
        }

        if (booking.Status != BookingStatus.Pending)
        {
            throw new InvalidOperationException("Only pending bookings can be confirmed.");
        }

        booking.Status = BookingStatus.Confirmed;
        await _dbContext.SaveChangesAsync();
    }

    public async Task CompleteAsync(int userId, int bookingId)
    {
        var booking = await _dbContext.Bookings
            .FirstOrDefaultAsync(b => b.Id == bookingId);

        if (booking == null)
        {
            throw new KeyNotFoundException("Booking not found.");
        }

        var isProvider = await _dbContext.Providers
            .AnyAsync(p => p.Id == booking.ProviderId && p.UserId == userId);

        if (!isProvider)
        {
            throw new UnauthorizedAccessException("Only the provider can complete this booking.");
        }

        if (booking.Status != BookingStatus.Confirmed)
        {
            throw new InvalidOperationException("Only confirmed bookings can be completed.");
        }

        booking.Status = BookingStatus.Completed;
        await _dbContext.SaveChangesAsync();
    }

    public async Task RejectAsync(int userId, int bookingId)
    {
        var booking = await _dbContext.Bookings
            .FirstOrDefaultAsync(b => b.Id == bookingId);

        if (booking == null)
        {
            throw new KeyNotFoundException("Booking not found.");
        }

        var isProvider = await _dbContext.Providers
            .AnyAsync(p => p.Id == booking.ProviderId && p.UserId == userId);

        if (!isProvider)
        {
            throw new UnauthorizedAccessException("Only the provider can reject this booking.");
        }

        if (booking.Status != BookingStatus.Pending)
        {
            throw new InvalidOperationException("Only pending bookings can be rejected.");
        }

        booking.Status = BookingStatus.Rejected;
        await _dbContext.SaveChangesAsync();
    }

    private static BookingResponseDto MapToDto(Booking booking)
    {
        return new BookingResponseDto
        {
            Id = booking.Id,
            CustomerId = booking.CustomerId,
            ProviderId = booking.ProviderId,
            ProviderServiceId = booking.ProviderServiceId,
            ServiceName = booking.ProviderService?.Name ?? string.Empty,
            ProviderName = booking.Provider?.User?.Username
                ?? booking.Provider?.DisplayName
                ?? string.Empty,
            BookingDate = booking.BookingDate,
            Status = booking.Status.ToString(),
            Note = booking.Note,
            TotalPrice = booking.TotalPrice,
            CreatedAt = booking.CreatedAt
        };
    }
}
