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
    private readonly IJwtService _jwtService;

    public AuthService(AppDbContext context, IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterRequestDto dto)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Email == dto.Email || u.Username == dto.Username);
        if (userExists)
            return null;

        var role = string.Equals(dto.Role, "Provider", StringComparison.OrdinalIgnoreCase) ? "Provider" : "Customer";

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = role
        };

        if (_context.Database.IsRelational())
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                if (role == "Provider")
                {
                    var provider = new Provider
                    {
                        UserId = user.Id,
                        DisplayName = user.Username
                    };
                    _context.Providers.Add(provider);
                }
                else
                {
                    var customer = new Customer
                    {
                        UserId = user.Id
                    };
                    _context.Customers.Add(customer);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        else
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            if (role == "Provider")
            {
                var provider = new Provider
                {
                    UserId = user.Id,
                    DisplayName = user.Username
                };
                _context.Providers.Add(provider);
            }
            else
            {
                var customer = new Customer
                {
                    UserId = user.Id
                };
                _context.Customers.Add(customer);
            }

            await _context.SaveChangesAsync();
        }

        var token = _jwtService.GenerateToken(user);

        return new AuthResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            Token = token
        };
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => (u.Email == dto.Email || u.Username == dto.Email) && !u.IsDeleted);
        if (user == null)
            return null;

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return null;

        var token = _jwtService.GenerateToken(user);

        return new AuthResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            Token = token
        };
    }

    public string GenerateJwtToken(AuthResponseDto user)
    {
        var dummyUser = new User
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            PasswordHash = string.Empty,
            Role = user.Role
        };
        return _jwtService.GenerateToken(dummyUser);
    }
}
