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

    public async Task<BookingResponseDto> CreateAsync(int userId, CreateBookingDto dto)
    {
        var customer = await _dbContext.Customers
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (customer == null)
        {
            throw new Exception("Customer not found.");
        }

        var providerService = await _dbContext.ProviderServices
            .Include(ps => ps.Provider)
            .FirstOrDefaultAsync(ps => ps.Id == dto.ProviderServiceId);

        if (providerService == null)
        {
            throw new Exception("Provider service not found.");
        }
        if(dto.BookingDate < DateTime.UtcNow)
        {
            throw new Exception("Booking date cannot be in the past.");
        }
        

        var booking = new Booking
        {
            CustomerId = customer.Id,
            ProviderId = providerService.ProviderId,
            ProviderServiceId = providerService.Id,
            BookingDate = dto.BookingDate,
            Note = dto.Note,
            Status = BookingStatus.Pending
        };

        _dbContext.Bookings.Add(booking);
        await _dbContext.SaveChangesAsync();

        return new BookingResponseDto
        {
            Id = booking.Id,
            CustomerId = booking.CustomerId,
            ProviderId = booking.ProviderId,
            ProviderServiceId = booking.ProviderServiceId,
            BookingDate = booking.BookingDate,
            Note = booking.Note,
            Status = booking.Status.ToString(),
            CreatedAt = booking.CreatedAt
        };
    }
    public async Task<IEnumerable<BookingResponseDto>> GetCustomerBookingsAsync(int userId)
    {
        var customer = await _dbContext.Customers
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (customer == null)
        {
            throw new Exception("Customer not found.");
        }

        var bookings = await _dbContext.Bookings
            .Where(b => b.CustomerId == customer.Id)
            .Include(b => b.ProviderService)
            .ThenInclude(ps => ps.Provider)
            .ToListAsync();

        return bookings.Select(b => new BookingResponseDto
        {
            Id = b.Id,
            CustomerId = b.CustomerId,
            ProviderId = b.ProviderId,
            ProviderServiceId = b.ProviderServiceId,
            ServiceName = b.ProviderService.Name,
            ProviderName = b.Provider.User.Username,
            BookingDate = b.BookingDate,
            Note = b.Note,
            Status = b.Status.ToString(),
            CreatedAt = b.CreatedAt
        });
    }
}