using ServiceBooking.Api.DTOs.Booking;

namespace ServiceBooking.Api.Services.Interfaces;

public interface IBookingService
{
    Task<BookingResponseDto> CreateAsync(
        int userId,
        CreateBookingDto dto);

    Task<IEnumerable<BookingResponseDto>> GetCustomerBookingsAsync(
        int userId);

    Task<BookingResponseDto?> GetByIdAsync(
        int userId,
        int bookingId);

    Task CancelAsync(
        int userId,
        int bookingId);

    Task ConfirmAsync(
        int userId,
        int bookingId);

    Task CompleteAsync(
        int userId,
        int bookingId);

    Task<IEnumerable<BookingResponseDto>> GetProviderBookingsAsync(
    int userId);
}