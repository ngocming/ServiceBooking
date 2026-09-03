using Microsoft.AspNetCore.Mvc;
using ServiceBooking.Api.DTOs.Providers;
using ServiceBooking.Api.Services.Interfaces;
using ServiceBooking.Api.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ServiceBooking.Api.Controllers;

[ApiController]
[Route("api/provider")]
public class ProvidersController : ControllerBase
{
    private readonly IProviderService _providerService;

    public ProvidersController(IProviderService providerService)
    {
        _providerService = providerService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<ProviderResponseDto>>> GetAll()
    {
        var providers = await _providerService.GetAllAsync();
        return Ok(providers);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProviderResponseDto>> GetById(int id)
    {
        var provider = await _providerService.GetByIdAsync(id);
        if (provider == null)
            return NotFound("Provider not found for get");
        return Ok(provider);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ProviderResponseDto>> Create([FromBody] CreateProviderDto dto)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("Invalid User ID.");
        }

        var provider = await _providerService.CreateAsync(dto, userId);

        if (provider == null)
            return BadRequest("Could not create provider. A provider profile may already exist for this user.");
        return CreatedAtAction(nameof(GetById), new { id = provider.Id }, provider);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<ProviderResponseDto>> Update(int id, [FromBody] UpdateProviderDto dto)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userRole = User.FindFirstValue(ClaimTypes.Role);
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("Invalid User ID.");
        }

        var existingProvider = await _providerService.GetByIdAsync(id);
        if (existingProvider == null)
            return NotFound("Provider not found for update");

        if (existingProvider.UserId != userId && userRole != "Admin")
        {
            return Forbid("You do not have permission to update this provider profile.");
        }

        var provider = await _providerService.UpdateAsync(id, dto);
        return Ok(provider);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> Delete(int id)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userRole = User.FindFirstValue(ClaimTypes.Role);
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("Invalid User ID.");
        }

        var existingProvider = await _providerService.GetByIdAsync(id);
        if (existingProvider == null)
            return NotFound("Provider not found for delete");

        if (existingProvider.UserId != userId && userRole != "Admin")
        {
            return Forbid("You do not have permission to delete this provider profile.");
        }

        var result = await _providerService.DeleteAsync(id);
        if (!result)
            return NotFound("Provider not found for delete");
        return NoContent();
    }
}