using ServiceBooking.Api.DTOs.ProviderService;
using ServiceBooking.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ServiceBooking.Api.Controllers;

[ApiController]
[Route("api/pvdservices")]
[Route("api/provider-services")]
public class ProviderServicesController : ControllerBase
{
    private readonly IProviderServiceOfferingService _providerServiceOfferingService;

    public ProviderServicesController(IProviderServiceOfferingService providerServiceOfferingService)
    {
        _providerServiceOfferingService = providerServiceOfferingService;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }

    [HttpGet("provider/{providerId:int}")]
    public async Task<ActionResult<List<ProviderServiceResponseDto>>> GetByProviderId(int providerId)
    {
        var providerServices = await _providerServiceOfferingService.GetByProviderIdAsync(providerId);
        return Ok(providerServices);
    }

    [HttpGet]
    public async Task<ActionResult<List<ProviderServiceResponseDto>>> GetAll()
    {
        var providerServices = await _providerServiceOfferingService.GetAllAsync() ?? new List<ProviderServiceResponseDto>();
        return Ok(providerServices);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProviderServiceResponseDto>> GetById(int id)
    {
        var providerService = await _providerServiceOfferingService.GetByIdAsync(id);
        if (providerService == null)
            return NotFound("Provider service not found.");
        return Ok(providerService);
    }

    [HttpPost]
    [Authorize(Roles = "Provider")]
    public async Task<ActionResult<ProviderServiceResponseDto>> Create([FromBody] CreateProviderServiceDto dto)
    {
        var userId = GetCurrentUserId();
        if (userId <= 0)
        {
            return Unauthorized("Invalid user token.");
        }

        var providerService = await _providerServiceOfferingService.CreateAsync(dto, userId);
        if (providerService == null)
        {
            return BadRequest("Could not create provider service.");
        }

        return CreatedAtAction(nameof(GetById), new { id = providerService.Id }, providerService);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Provider")]
    public async Task<ActionResult<ProviderServiceResponseDto>> Update(int id, [FromBody] UpdateProviderServiceDto dto)
    {
        var userId = GetCurrentUserId();
        if (userId <= 0)
        {
            return Unauthorized("Invalid user token.");
        }

        var result = await _providerServiceOfferingService.UpdateAsync(userId, id, dto);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Provider")]
    public async Task<ActionResult> Delete(int id)
    {
        var userId = GetCurrentUserId();
        if (userId <= 0)
        {
            return Unauthorized("Invalid user token.");
        }

        await _providerServiceOfferingService.DeleteAsync(userId, id);
        return NoContent();
    }
}
