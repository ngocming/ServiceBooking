using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceBooking.Api.DTOs.Availability;
using ServiceBooking.Api.Services.Interfaces;
using System.Security.Claims;

namespace ServiceBooking.Api.Controllers;

[ApiController]
[Route("api/availability")]
public class AvailabilityController : ControllerBase
{
    private readonly IAvailabilityService _availabilityService;

    public AvailabilityController(IAvailabilityService availabilityService)
    {
        _availabilityService = availabilityService;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }

    [HttpGet("providers/{providerId:int}")]
    public async Task<ActionResult<ProviderScheduleResponseDto>> GetProviderSchedule(int providerId)
    {
        var schedule = await _availabilityService.GetProviderScheduleAsync(providerId);
        return Ok(schedule);
    }

    [HttpGet("my-schedule")]
    [Authorize(Roles = "Provider")]
    public async Task<ActionResult<ProviderScheduleResponseDto>> GetMySchedule()
    {
        var userId = GetCurrentUserId();
        if (userId <= 0)
        {
            return Unauthorized("Invalid user token.");
        }

        var schedule = await _availabilityService.GetMyScheduleAsync(userId);
        return Ok(schedule);
    }

    [HttpPut("my-schedule")]
    [Authorize(Roles = "Provider")]
    public async Task<IActionResult> SetWeeklySchedule([FromBody] SetWeeklyAvailabilityDto dto)
    {
        var userId = GetCurrentUserId();
        if (userId <= 0)
        {
            return Unauthorized("Invalid user token.");
        }

        await _availabilityService.SetWeeklyScheduleAsync(userId, dto);
        return Ok(new { message = "Weekly schedule updated successfully." });
    }

    [HttpPost("exceptions")]
    [Authorize(Roles = "Provider")]
    public async Task<ActionResult<AvailabilityExceptionDto>> AddException([FromBody] CreateAvailabilityExceptionDto dto)
    {
        var userId = GetCurrentUserId();
        if (userId <= 0)
        {
            return Unauthorized("Invalid user token.");
        }

        var created = await _availabilityService.AddExceptionAsync(userId, dto);
        return Ok(created);
    }

    [HttpDelete("exceptions/{id:int}")]
    [Authorize(Roles = "Provider")]
    public async Task<IActionResult> DeleteException(int id)
    {
        var userId = GetCurrentUserId();
        if (userId <= 0)
        {
            return Unauthorized("Invalid user token.");
        }

        await _availabilityService.DeleteExceptionAsync(userId, id);
        return Ok(new { message = "Availability exception deleted successfully." });
    }
}
