using ServiceBooking.Api.Data;
using ServiceBooking.Api.DTOs.Providers;
using ServiceBooking.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ServiceBooking.Api.Models;

namespace ServiceBooking.Api.Services.Implementations;

public class Provider_Service : IProviderService
{
    private readonly AppDbContext _context;

    public Provider_Service(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProviderResponseDto>> GetAllAsync()
    {
        return await _context.Providers
            .Select(p => new ProviderResponseDto
            {
                Id = p.Id,
                UserId = p.UserId,
                DisplayName = p.DisplayName,
                Description = p.Description,
                Phone = p.Phone,
                Address = p.Address,
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                IsAvailable = p.IsAvailable,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<ProviderResponseDto?> GetByIdAsync(int id)
    {
        return await _context.Providers
            .Where(p => p.Id == id)
            .Select(p => new ProviderResponseDto
            {
                Id = p.Id,
                UserId = p.UserId,
                DisplayName = p.DisplayName,
                Description = p.Description,
                Phone = p.Phone,
                Address = p.Address,
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                IsAvailable = p.IsAvailable,
                CreatedAt = p.CreatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ProviderResponseDto?> CreateAsync(CreateProviderDto dto, int userId)
    {
        var existingProvider = await _context.Providers.FirstOrDefaultAsync(p => p.UserId == userId);
        if (existingProvider != null)
        {
            return null; 
        }

        var provider = new Provider
        {
            UserId = userId,
            DisplayName = dto.DisplayName,
            Description = dto.Description,
            Phone = dto.Phone,
            Address = dto.Address,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            IsAvailable = dto.IsAvailable
        };

        _context.Providers.Add(provider);
        await _context.SaveChangesAsync();

        return new ProviderResponseDto
        {
            Id = provider.Id,
            UserId = provider.UserId,
            DisplayName = provider.DisplayName,
            Description = provider.Description,
            Phone = provider.Phone,
            Address = provider.Address,
            Latitude = provider.Latitude,
            Longitude = provider.Longitude,
            IsAvailable = provider.IsAvailable,
            CreatedAt = provider.CreatedAt
        };
    }

    public async Task<ProviderResponseDto?> UpdateAsync(int id, UpdateProviderDto dto)
    {
        var provider = await _context.Providers.FindAsync(id);
        if (provider == null)
            return null;

        provider.DisplayName = dto.DisplayName;
        provider.Description = dto.Description;
        provider.Phone = dto.Phone;
        provider.Address = dto.Address;
        provider.Latitude = dto.Latitude;
        provider.Longitude = dto.Longitude;
        provider.IsAvailable = dto.IsAvailable;

        await _context.SaveChangesAsync();

        return new ProviderResponseDto
        {
            Id = provider.Id,
            UserId = provider.UserId,
            DisplayName = provider.DisplayName,
            Description = provider.Description,
            Phone = provider.Phone,
            Address = provider.Address,
            Latitude = provider.Latitude,
            Longitude = provider.Longitude,
            IsAvailable = provider.IsAvailable,
            CreatedAt = provider.CreatedAt
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var provider = await _context.Providers.FindAsync(id);
        if (provider == null)
            return false;

        _context.Providers.Remove(provider);
        await _context.SaveChangesAsync();

        return true;
    }
}