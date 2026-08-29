using Microsoft.AspNetCore.Mvc;
using ServiceBooking.Api.DTOs.Auth;
using ServiceBooking.Api.Services.Interfaces;

namespace ServiceBooking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto dto)
    {
        var user = await _authService.RegisterAsync(dto);
        if (user == null)
            return BadRequest("Could not register user");
        return Ok(user);
    }
}