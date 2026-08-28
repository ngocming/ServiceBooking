using Microsoft.AspNetCore.Mvc;
using ServiceBooking.Api.DTOs.User;
using ServiceBooking.Api.Services.Interfaces;

namespace ServiceBooking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    
    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpGet]
    public async Task<ActionResult<List<UserResponseDto>>> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponseDto>> GetById(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null)
            return NotFound("User not found");
        return Ok(user);
    }
    
    [HttpPost]
    public async Task<ActionResult<UserResponseDto>> Create([FromBody] CreateUserDto dto)
    {
        var user = await _userService.CreateAsync(dto);
        if (user == null)
            return BadRequest("Could not create user");
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult<UserResponseDto>> Update(int id, [FromBody] UpdateUserDto dto)
    {
        var user = await _userService.UpdateAsync(id, dto);
        if (user == null)
            return NotFound("User not found for update");
        return Ok(user);
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _userService.DeleteAsync(id);
        if (!result)
            return NotFound("User not found for delete");
        return NoContent();
    }
}