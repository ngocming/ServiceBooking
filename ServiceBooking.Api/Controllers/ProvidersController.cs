using Microsoft.AspNetCore.Mvc;
using ServiceBooking.Api.DTOs.Providers;
using ServiceBooking.Api.Services.Interfaces;
using ServiceBooking.Api.Models;

namespace ServiceBooking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProvidersController : ControllerBase
{
    private readonly IProviderService _providerService;

    public ProvidersController(IProviderService providerService)
    {
        _providerService = providerService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProviderResponseDto>>> GetAll()
    {
        var providers = await _providerService.GetAllAsync();
        return Ok(providers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProviderResponseDto>> GetById(int id)
    {
        var provider = await _providerService.GetByIdAsync(id);
        if (provider == null)
            return NotFound("Provider not found for get");
        return Ok(provider);
    }

    [HttpPost]
    public async Task<ActionResult<ProviderResponseDto>> Create([FromBody] CreateProviderDto dto)
    {
        var provider = await _providerService.CreateAsync(dto);
        if (provider == null)
            return BadRequest("Could not create provider");
        return CreatedAtAction(nameof(GetById), new { id = provider.Id }, provider);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProviderResponseDto>> Update(int id, [FromBody] UpdateProviderDto dto)
    {
        var provider = await _providerService.UpdateAsync(id, dto);
        if (provider == null)
            return NotFound("Provider not found for update");
        return Ok(provider);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _providerService.DeleteAsync(id);
        if (!result)
            return NotFound("Provider not found for delete");
        return NoContent();
    }
}