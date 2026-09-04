using ServiceBooking.Api.Data;
using ServiceBooking.Api.DTOs.Customers;
using ServiceBooking.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ServiceBooking.Api.Models;

namespace ServiceBooking.Api.Services.Implementations;

public class CustomerService : ICustomerService
{
    private readonly AppDbContext _context;

    public CustomerService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CustomerResponseDto?> GetMyProfileAsync(int userId)
    {
        return await _context.Customers
            .Where(c => c.UserId == userId)
            .Select(c => new CustomerResponseDto
            {
                Id = c.Id,
                UserId = c.UserId,
                Username = c.User.Username,
                Email = c.User.Email,
                IsDeleted = c.User.IsDeleted,
                CreatedAt = c.CreatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<CustomerResponseDto?> CreateAsync(int userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            return null; // User does not exist
        }
        var existingCustomer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == userId);
        if (existingCustomer != null)
        {
            return await GetMyProfileAsync(userId); 
        }

        var customer = new Customer
        {
            UserId = userId,
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        return await GetMyProfileAsync(userId);
    }
    public async Task<CustomerResponseDto?> UpdateAsync(int userId, UpdateCustomerDto dto)
    {
        var customer = await _context.Customers
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.UserId == userId);
        if (customer == null)
        {
            return null; 
        }

        if (customer.User != null)
        {
            if (!string.IsNullOrWhiteSpace(dto.Username))
                customer.User.Username = dto.Username;
            if (!string.IsNullOrWhiteSpace(dto.Email))
                customer.User.Email = dto.Email;
        }

        await _context.SaveChangesAsync();

        return await GetMyProfileAsync(userId);
    }

    public async Task<List<CustomerResponseDto>> GetAllAsync()
    {
        return await _context.Customers
            .Select(c => new CustomerResponseDto
            {
                Id = c.Id,
                UserId = c.UserId,
                Username = c.User.Username,
                Email = c.User.Email,
                IsDeleted = c.User.IsDeleted,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync();
    }
}