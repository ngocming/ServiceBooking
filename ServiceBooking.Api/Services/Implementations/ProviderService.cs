using ServiceBooking.Api.Data;
using ServiceBooking.Api.DTOs.Availability;
using ServiceBooking.Api.DTOs.Providers;
using ServiceBooking.Api.DTOs.ProviderService;
using ServiceBooking.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ServiceBooking.Api.Models;

namespace ServiceBooking.Api.Services.Implementations;

public class ProviderService : IProviderService
{
    private readonly AppDbContext _context;

    public ProviderService(AppDbContext context)
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

    public async Task<ProviderPublicProfileDto?> GetPublicProfileAsync(int id)
    {
        var provider = await _context.Providers
            .Include(p => p.User)
            .Include(p => p.ProviderServices.Where(s => s.IsAvailable)).ThenInclude(s => s.Category)
            .Include(p => p.Availabilities.Where(a => a.IsAvailable))
            .FirstOrDefaultAsync(p => p.Id == id);

        if (provider == null)
            return null;

        var completedCount = await _context.Bookings
            .CountAsync(b => b.ProviderId == id && b.Status == BookingStatus.Completed);

        return new ProviderPublicProfileDto
        {
            Id = provider.Id,
            DisplayName = provider.DisplayName ?? provider.User?.Username ?? string.Empty,
            Description = provider.Description,
            Phone = provider.Phone,
            Address = provider.Address,
            Latitude = provider.Latitude,
            Longitude = provider.Longitude,
            IsAvailable = provider.IsAvailable,
            CompletedBookingsCount = completedCount,
            Services = provider.ProviderServices.Select(s => new ProviderServiceResponseDto
            {
                Id = s.Id,
                ProviderId = s.ProviderId,
                ProviderName = provider.DisplayName ?? provider.User?.Username,
                CategoryId = s.CategoryId,
                CategoryName = s.Category?.Name,
                Name = s.Name,
                Description = s.Description,
                ImageUrl = s.ImageUrl,
                Price = s.Price,
                DurationMinutes = s.DurationMinutes,
                IsAvailable = s.IsAvailable,
                CreatedAt = s.CreatedAt
            }).ToList(),
            WeeklySchedule = provider.Availabilities
                .OrderBy(a => a.DayOfWeek)
                .ThenBy(a => a.StartTime)
                .Select(a => new ProviderAvailabilityDto
                {
                    Id = a.Id,
                    DayOfWeek = a.DayOfWeek,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    IsAvailable = a.IsAvailable
                }).ToList()
        };
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
    public async Task<ProviderResponseDto?> GetByUserIdAsync(int userId)
    {
        return await _context.Providers
            .Where(p => p.UserId == userId)
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
}
