using ServiceBooking.Api.Data;
using ServiceBooking.Api.DTOs.User;
using ServiceBooking.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ServiceBooking.Api.Models;

namespace ServiceBooking.Api.Services.Implementations;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    
    public UserService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<UserResponseDto>> GetAllAsync()
    {
        return await _context.Users
            .Select(u => new UserResponseDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                FullName = u.FullName,
                PhoneNumber = u.PhoneNumber,
                Role = u.Role,
                CreatedAt = u.CreatedAt
            })
            .ToListAsync();
    }
    
    public async Task<UserResponseDto?> GetByIdAsync(int id)
    {
        return await _context.Users
            .Where(u => u.Id == id)
            .Select(u => new UserResponseDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                FullName = u.FullName,
                PhoneNumber = u.PhoneNumber,
                Role = u.Role,
                CreatedAt = u.CreatedAt
            })
            .FirstOrDefaultAsync();
    }
    
    public async Task<UserResponseDto?> CreateAsync(CreateUserDto dto)
    {
        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = dto.Password,
            FullName = dto.FullName,
            PhoneNumber = dto.PhoneNumber,
            Role = dto.Role
        };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        return new UserResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };
    }
    
    public async Task<UserResponseDto?> UpdateAsync(int id, UpdateUserDto dto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return null;
        
        user.Username = dto.Username;
        user.PasswordHash = dto.Password;
        user.Email = dto.Email;
        user.FullName = dto.FullName;
        user.PhoneNumber = dto.PhoneNumber;
        user.Role = dto.Role;
        
        await _context.SaveChangesAsync();
        
        return new UserResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };
    }
    
    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return false;
        
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        
        return true;
    }
}
