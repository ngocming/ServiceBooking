using Microsoft.AspNetCore.Mvc;
using ServiceBooking.Api.DTOs.Booking;
using ServiceBooking.Api.Services.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace ServiceBooking.Api.Controllers;

[ApiController]
[Route("api/bookings")]
[Authorize]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }

    [HttpPost]
    public async Task<ActionResult<BookingResponseDto>> CreateBooking([FromBody] CreateBookingDto dto)
    {
        var userId = GetCurrentUserId();
        if (userId <= 0)
        {
            return Unauthorized("Invalid User Token.");
        }

        var booking = await _bookingService.CreateAsync(userId, dto);
        return Ok(booking);
    }

    [HttpGet("customer")]
    public async Task<ActionResult<IEnumerable<BookingResponseDto>>> GetCustomerBookings()
    {
        var userId = GetCurrentUserId();
        if (userId <= 0)
        {
            return Unauthorized("Invalid User Token.");
        }

        var bookings = await _bookingService.GetCustomerBookingsAsync(userId);
        return Ok(bookings);
    }
}