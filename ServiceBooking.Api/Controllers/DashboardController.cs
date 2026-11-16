using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceBooking.Api.Data;
using ServiceBooking.Api.Models;
using System.Security.Claims;

namespace ServiceBooking.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public DashboardController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }

    [HttpGet("provider")]
    public async Task<IActionResult> GetProviderDashboard()
    {
        var userId = GetCurrentUserId();
        if (userId <= 0)
            return Unauthorized("Invalid user token.");

        var provider = await _dbContext.Providers
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (provider == null)
            return NotFound("Provider profile not found.");

        var totalServices = await _dbContext.ProviderServices
            .CountAsync(ps => ps.ProviderId == provider.Id);

        var pendingBookings = await _dbContext.Bookings
            .CountAsync(b => b.ProviderId == provider.Id && b.Status == BookingStatus.Pending);

        var confirmedBookings = await _dbContext.Bookings
            .CountAsync(b => b.ProviderId == provider.Id && b.Status == BookingStatus.Confirmed);

        var completedBookings = await _dbContext.Bookings
            .CountAsync(b => b.ProviderId == provider.Id && b.Status == BookingStatus.Completed);

        var totalRevenue = await _dbContext.Bookings
            .Where(b => b.ProviderId == provider.Id && b.Status == BookingStatus.Completed)
            .SumAsync(b => b.TotalPrice);

        return Ok(new
        {
            totalServices,
            pendingBookings,
            confirmedBookings,
            completedBookings,
            totalRevenue
        });
    }
}
