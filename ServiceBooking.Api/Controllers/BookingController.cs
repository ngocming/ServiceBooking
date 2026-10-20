using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceBooking.Api.DTOs.Booking;
using ServiceBooking.Api.Services.Interfaces;
using System.Security.Claims;

namespace ServiceBooking.Api.Controllers;

[ApiController]
[Route("api/booking")]
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
            return Unauthorized("Invalid user token.");
        }

        var booking = await _bookingService.CreateAsync(userId, dto);
        return CreatedAtAction(
            nameof(GetBookingById),
            new { id = booking.Id },
            booking);
    }

    [Authorize(Roles = "Customer")]
    [HttpGet("customer/bookings")]
    public async Task<ActionResult<IEnumerable<BookingResponseDto>>> GetCustomerBookings()
    {
        var userId = GetCurrentUserId();
        if (userId <= 0)
        {
            return Unauthorized("Invalid user token.");
        }

        var bookings = await _bookingService.GetCustomerBookingsAsync(userId);
        return Ok(bookings);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookingResponseDto>> GetBookingById(int id)
    {
        var userId = GetCurrentUserId();
        if (userId <= 0)
        {
            return Unauthorized("Invalid user token.");
        }

        var booking = await _bookingService.GetByIdAsync(userId, id);
        if (booking == null)
        {
            return NotFound("Booking not found.");
        }

        return Ok(booking);
    }

    [HttpPatch("{id:int}/cancel")]
    public async Task<IActionResult> CancelBooking(int id)
    {
        var userId = GetCurrentUserId();
        if (userId <= 0)
        {
            return Unauthorized("Invalid user token.");
        }

        await _bookingService.CancelAsync(userId, id);
        return Ok(new { message = "Booking cancelled successfully." });
    }

    [HttpPatch("{id:int}/confirm")]
    public async Task<IActionResult> ConfirmBooking(int id)
    {
        var userId = GetCurrentUserId();
        if (userId <= 0)
        {
            return Unauthorized("Invalid user token.");
        }

        await _bookingService.ConfirmAsync(userId, id);
        return Ok(new { message = "Booking confirmed successfully." });
    }

    [HttpPatch("{id:int}/complete")]
    public async Task<IActionResult> CompleteBooking(int id)
    {
        var userId = GetCurrentUserId();
        if (userId <= 0)
        {
            return Unauthorized("Invalid user token.");
        }

        await _bookingService.CompleteAsync(userId, id);
        return Ok(new { message = "Booking completed successfully." });
    }

    [HttpGet("provider/bookings")]
    public async Task<ActionResult<IEnumerable<BookingResponseDto>>> GetProviderBookings()
    {
        var userId = GetCurrentUserId();
        if (userId <= 0)
        {
            return Unauthorized("Invalid user token.");
        }

        var bookings = await _bookingService.GetProviderBookingsAsync(userId);
        return Ok(bookings);
    }
}