using ServiceBooking.Api.DTOs.ProviderService;
using ServiceBooking.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

namespace ServiceBooking.Api.Controllers;

[ApiController]
[Route("api/provider-services")]
public class ProviderServicesController : ControllerBase
{
    private readonly IProviderServiceOfferingService _providerServiceOfferingService;

    public ProviderServicesController(IProviderServiceOfferingService providerServiceOfferingService)
    {
        _providerServiceOfferingService = providerServiceOfferingService;
    }
    [HttpGet("provider/{providerId}")]
    public async Task<ActionResult<List<ProviderServiceResponseDto>>> GetByProviderId(int providerId)
    {
        var providerServices = await _providerServiceOfferingService.GetByProviderIdAsync(providerId);
        return Ok(providerServices);
    }
    [HttpGet]
    public async Task<ActionResult<List<ProviderServiceResponseDto>>> GetAll()
    {
        var providerServices = await _providerServiceOfferingService.GetAllAsync();
        return Ok(providerServices);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProviderServiceResponseDto>> GetById(int id)
    {
        var providerService = await _providerServiceOfferingService.GetByIdAsync(id);
        if (providerService == null)
            return NotFound("Provider service not found");
        return Ok(providerService);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ProviderServiceResponseDto>> Create([FromBody] CreateProviderServiceDto dto)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("Invalid User ID.");
        }

        var providerService = await _providerServiceOfferingService.CreateAsync(dto, userId);

        if (providerService == null)
            return BadRequest("Could not create provider service");
        return CreatedAtAction(nameof(GetById), new { id = providerService.Id }, providerService);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> Delete(int id)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("Invalid User ID.");
        }

        var result = await _providerServiceOfferingService.DeleteAsync(userId, id);
        if (!result)
            return NotFound("Provider service not found or you do not have permission to delete it.");
        return NoContent();
    }
}