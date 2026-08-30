using Microsoft.AspNetCore.Mvc;
using ServiceBooking.Api.DTOs.Auth;
using ServiceBooking.Api.Services.Interfaces;

namespace ServiceBooking.Api.Controllers;

[ApiController]
[Route("api/auth")]
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
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto dto)
    {
        var user = await _authService.LoginAsync(dto);
        if (user == null)
            return BadRequest("Invalid email or password");
        return Ok(user);
    }
}