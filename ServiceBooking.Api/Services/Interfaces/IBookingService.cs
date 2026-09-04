using ServiceBooking.Api.DTOs.Booking;

namespace ServiceBooking.Api.Services.Interfaces;

public interface IBookingService
{
    Task<BookingResponseDto> CreateAsync(
        int userId,
        CreateBookingDto dto);

    Task<IEnumerable<BookingResponseDto>> GetCustomerBookingsAsync(
        int userId);
}