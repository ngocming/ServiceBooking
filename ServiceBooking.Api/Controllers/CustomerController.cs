using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceBooking.Api.DTOs.Customers;
using ServiceBooking.Api.Services.Interfaces;

namespace ServiceBooking.Api.Controllers;

[ApiController]
[Route("api/customers")]
[Authorize]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }   

    [HttpGet("profile")]
    public async Task<ActionResult<CustomerResponseDto>> GetMyProfile()
    {
        var userId = GetCurrentUserId();
        if (userId <= 0)
            return Unauthorized("Invalid User Token.");

        var profile = await _customerService.GetMyProfileAsync(userId);
        if (profile == null)
            return NotFound("Customer profile not found");
        return Ok(profile);
    }

    [HttpPost("create")]
    public async Task<ActionResult<CustomerResponseDto>> Create()
    {
        var userId = GetCurrentUserId();
        if (userId <= 0)
            return Unauthorized("Invalid User Token.");

        var customer = await _customerService.CreateAsync(userId);
        if (customer == null)
            return BadRequest("Could not create customer profile");
        return Ok(customer);
    }

    [HttpPut("update")]
    public async Task<ActionResult<CustomerResponseDto>> Update([FromBody] CustomerResponseDto dto)
    {
        var userId = GetCurrentUserId();
        if (userId <= 0)
            return Unauthorized("Invalid User Token.");

        var updatedCustomer = await _customerService.UpdateAsync(userId, dto);
        if (updatedCustomer == null)
            return NotFound("Customer profile not found");
        return Ok(updatedCustomer);
    }
}