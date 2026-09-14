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
        var userIdClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        return int.TryParse(userIdClaim, out var userId)
            ? userId
            : 0;
    }

    [HttpPost]
    public async Task<ActionResult<BookingResponseDto>>
        CreateBooking([FromBody] CreateBookingDto dto)
    {
        var userId = GetCurrentUserId();

        if (userId <= 0)
        {
            return Unauthorized("Invalid user token.");
        }

        try
        {
            var booking =
                await _bookingService.CreateAsync(userId, dto);

            return CreatedAtAction(
                nameof(GetBookingById),
                new { id = booking.Id },
                booking);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpGet("customer/bookings")]
    public async Task<ActionResult<IEnumerable<BookingResponseDto>>>
        GetCustomerBookings()
    {
        var userId = GetCurrentUserId();

        if (userId <= 0)
        {
            return Unauthorized("Invalid user token.");
        }

        try
        {
            var bookings =
                await _bookingService
                    .GetCustomerBookingsAsync(userId);

            return Ok(bookings);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookingResponseDto>>
        GetBookingById(int id)
    {
        var userId = GetCurrentUserId();

        if (userId <= 0)
        {
            return Unauthorized("Invalid user token.");
        }

        try
        {
            var booking =
                await _bookingService.GetByIdAsync(userId, id);

            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            return Ok(booking);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpPatch("{id:int}/cancel")]
    public async Task<IActionResult> CancelBooking(int id)
    {
        var userId = GetCurrentUserId();

        if (userId <= 0)
        {
            return Unauthorized("Invalid user token.");
        }

        try
        {
            await _bookingService.CancelAsync(userId, id);

            return Ok(new
            {
                message = "Booking cancelled successfully."
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPatch("{id:int}/confirm")]
    public async Task<IActionResult> ConfirmBooking(int id)
    {
        var userId = GetCurrentUserId();

        if (userId <= 0)
        {
            return Unauthorized("Invalid user token.");
        }

        try
        {
            await _bookingService.ConfirmAsync(userId, id);

            return Ok(new
            {
                message = "Booking confirmed successfully."
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPatch("{id:int}/complete")]
    public async Task<IActionResult> CompleteBooking(int id)
    {
        var userId = GetCurrentUserId();

        if (userId <= 0)
        {
            return Unauthorized("Invalid user token.");
        }

        try
        {
            await _bookingService.CompleteAsync(userId, id);

            return Ok(new
            {
                message = "Booking completed successfully."
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }
    [HttpGet("provider/bookings")]
    public async Task<ActionResult<IEnumerable<BookingResponseDto>>>
        GetProviderBookings()
    {
        var userId = GetCurrentUserId();

        if (userId <= 0)
        {
            return Unauthorized("Invalid user token.");
        }

        try
        {
            var bookings =
                await _bookingService.GetProviderBookingsAsync(userId);

            return Ok(bookings);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}