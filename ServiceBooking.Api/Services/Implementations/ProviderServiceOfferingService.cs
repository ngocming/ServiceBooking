using ServiceBooking.Api.Data;
using ServiceBooking.Api.DTOs.ProviderService;
using ServiceBooking.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ServiceBooking.Api.Models;

namespace ServiceBooking.Api.Services.Implementations;

public class ProviderServiceOfferingService : IProviderServiceOfferingService
{
    private readonly AppDbContext _context;

    public ProviderServiceOfferingService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<ProviderServiceResponseDto>> GetByProviderIdAsync(int providerId)
    {
        return await _context.ProviderServices
            .Where(pso => pso.ProviderId == providerId)
            .Select(pso => new ProviderServiceResponseDto
            {
                Id = pso.Id,
                ProviderId = pso.ProviderId,
                Name = pso.Name,
                Description = pso.Description,
                Price = pso.Price,
                DurationMinutes = pso.DurationMinutes,
                IsAvailable = pso.IsAvailable,
                CreatedAt = pso.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<ProviderServiceResponseDto?> GetByIdAsync(int id)
    {
        return await _context.ProviderServices
            .Where(pso => pso.Id == id)
            .Select(pso => new ProviderServiceResponseDto
            {
                Id = pso.Id,
                ProviderId = pso.ProviderId,
                Name = pso.Name,
                Description = pso.Description,
                Price = pso.Price,
                DurationMinutes = pso.DurationMinutes,
                IsAvailable = pso.IsAvailable,
                CreatedAt = pso.CreatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ProviderServiceResponseDto?> CreateAsync(CreateProviderServiceDto dto, int userId)
    {
        var provider = await _context.Providers.FirstOrDefaultAsync(p => p.UserId == userId);
        if (provider == null)
        {
            throw new UnauthorizedAccessException("User is not registered as a provider.");
        }

        var providerService = new Models.ProviderService
        {
            ProviderId = provider.Id,
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            DurationMinutes = dto.DurationMinutes,
            IsAvailable = true
        };

        _context.ProviderServices.Add(providerService);
        await _context.SaveChangesAsync();

        return new ProviderServiceResponseDto
        {
            Id = providerService.Id,
            ProviderId = providerService.ProviderId,
            Name = providerService.Name,
            Description = providerService.Description,
            Price = providerService.Price,
            DurationMinutes = providerService.DurationMinutes,
            IsAvailable = providerService.IsAvailable,
            CreatedAt = providerService.CreatedAt
        };
    }

    public async Task<ProviderServiceResponseDto?> UpdateAsync(int userId, int id, UpdateProviderServiceDto dto)
    {
        var provider = await _context.Providers.FirstOrDefaultAsync(p => p.UserId == userId);
        if (provider == null)
        {
            throw new UnauthorizedAccessException("User is not registered as a provider.");
        }

        var serviceOffering = await _context.ProviderServices.FirstOrDefaultAsync(pso => pso.Id == id);
        if (serviceOffering == null)
        {
            throw new KeyNotFoundException("Provider service not found.");
        }

        if (serviceOffering.ProviderId != provider.Id)
        {
            throw new UnauthorizedAccessException("You do not have permission to update this service.");
        }

        serviceOffering.Name = dto.Name;
        serviceOffering.Description = dto.Description;
        serviceOffering.Price = dto.Price;
        serviceOffering.DurationMinutes = dto.DurationMinutes;
        serviceOffering.IsAvailable = dto.IsAvailable;

        await _context.SaveChangesAsync();

        return new ProviderServiceResponseDto
        {
            Id = serviceOffering.Id,
            ProviderId = serviceOffering.ProviderId,
            Name = serviceOffering.Name,
            Description = serviceOffering.Description,
            Price = serviceOffering.Price,
            DurationMinutes = serviceOffering.DurationMinutes,
            IsAvailable = serviceOffering.IsAvailable,
            CreatedAt = serviceOffering.CreatedAt
        };
    }

    public async Task<bool> DeleteAsync(int userId, int id)
    {
        var provider = await _context.Providers.FirstOrDefaultAsync(p => p.UserId == userId);
        if (provider == null)
        {
            throw new UnauthorizedAccessException("User is not registered as a provider.");
        }

        var serviceOffering = await _context.ProviderServices.FirstOrDefaultAsync(pso => pso.Id == id);
        if (serviceOffering == null)
        {
            throw new KeyNotFoundException("Provider service not found.");
        }

        if (serviceOffering.ProviderId != provider.Id)
        {
            throw new UnauthorizedAccessException("You do not have permission to delete this service.");
        }

        _context.ProviderServices.Remove(serviceOffering);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<ProviderServiceResponseDto>?> GetAllAsync()
    {
        var providerServices = await _context.ProviderServices.ToListAsync();
        if (providerServices == null || !providerServices.Any())
        {
            return new List<ProviderServiceResponseDto>();
        }

        return providerServices.Select(pso => new ProviderServiceResponseDto
        {
            Id = pso.Id,
            ProviderId = pso.ProviderId,
            Name = pso.Name,
            Description = pso.Description,
            Price = pso.Price,
            DurationMinutes = pso.DurationMinutes,
            IsAvailable = pso.IsAvailable,
            CreatedAt = pso.CreatedAt
        }).ToList();
    }
}
