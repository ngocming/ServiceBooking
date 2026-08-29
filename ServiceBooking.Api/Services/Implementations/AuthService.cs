using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ServiceBooking.Api.Data;
using ServiceBooking.Api.DTOs.Auth;
using ServiceBooking.Api.Models;
using ServiceBooking.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ServiceBooking.Api.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    
    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterRequestDto dto)
    {
        var emailExit = await _context.Users.AnyAsync(u => u.Email == dto.Email);
        if(emailExit)
            return null;

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = "Customer"
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role
        };
    }
}