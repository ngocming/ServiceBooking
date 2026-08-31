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
    private readonly IProviderService_OS _providerService;

    public ProviderServicesController(IProviderService_OS providerService)
    {
        _providerService = providerService;
    }
    [HttpGet("provider/{providerId}")]
    public async Task<ActionResult<List<ProviderServiceResponseDto>>> GetByProviderId(int providerId)
    {
        var providerServices = await _providerService.GetByProviderIdAsync(providerId);
        return Ok(providerServices);
    }
    [HttpGet]
    public async Task<ActionResult<List<ProviderServiceResponseDto>>> GetAll()
    {
        var providerServices = await _providerService.GetAllAsync();
        return Ok(providerServices);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProviderServiceResponseDto>> GetById(int id)
    {
        var providerService = await _providerService.GetByIdAsync(id);
        if (providerService == null)
            return NotFound("Provider service not found");
        return Ok(providerService);
    }

    [HttpPost]
    public async Task<ActionResult<ProviderServiceResponseDto>> Create([FromBody] CreateProviderServiceDto dto)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("Invalid User ID.");
        }

        var providerService = await _providerService.CreateAsync(dto, userId);

        if (providerService == null)
            return BadRequest("Could not create provider service");
        return CreatedAtAction(nameof(GetById), new { id = providerService.Id }, providerService);
    }


    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id, [FromQuery] int providerId)
    {
        var result = await _providerService.DeleteAsync(providerId, id);
        if (!result)
            return NotFound("Provider service not found for delete");
        return NoContent();
    }
}