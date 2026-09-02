using Microsoft.AspNetCore.Mvc;
using ServiceBooking.Api.DTOs.Customers;
using ServiceBooking.Api.Services.Interfaces;

namespace ServiceBooking.Api.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet("profile")]
    public async Task<ActionResult<CustomerResponseDto?>> GetMyProfile([FromQuery] int userId)
    {
        var profile = await _customerService.GetMyProfileAsync(userId);
        if (profile == null)
            return NotFound("Customer profile not found");
        return Ok(profile);
    }

    [HttpPost("create")]
    public async Task<ActionResult<CustomerResponseDto?>> Create([FromQuery] int userId)
    {
        var customer = await _customerService.CreateAsync(userId);
        if (customer == null)
            return BadRequest("Could not create customer profile");
        return Ok(customer);
    }

    [HttpPut("update")]
    public async Task<ActionResult<CustomerResponseDto?>> Update([FromQuery] int userId, [FromBody] CustomerResponseDto dto)
    {
        var updatedCustomer = await _customerService.UpdateAsync(userId, dto);
        if (updatedCustomer == null)
            return NotFound("Customer profile not found");
        return Ok(updatedCustomer);
    }
}