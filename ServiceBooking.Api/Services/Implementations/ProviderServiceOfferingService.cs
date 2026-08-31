using ServiceBooking.Api.Data;
using ServiceBooking.Api.DTOs.ProviderService;
using ServiceBooking.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ServiceBooking.Api.Models;

namespace ServiceBooking.Api.Services.Implementations;
public class ProviderService_OS : IProviderService_OS
{
    private readonly AppDbContext _context;

    public ProviderService_OS(AppDbContext context)
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
                DurationInMinutes = pso.DurationMinutes,
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
                DurationInMinutes = pso.DurationMinutes,
                CreatedAt = pso.CreatedAt
            })
            .FirstOrDefaultAsync();
    }
    public async Task<ProviderServiceResponseDto?> CreateAsync(CreateProviderServiceDto dto, int userId)
    {
        var provider = await _context.Providers.FirstOrDefaultAsync(p => p.UserId == userId);
        if (provider == null)
        {
            return null;
        }
        var providerService = new ProviderService
        {
            ProviderId = provider.Id,
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            DurationMinutes = dto.DurationMinutes
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
            DurationInMinutes = providerService.DurationMinutes,
            CreatedAt = providerService.CreatedAt
        };
    }
    public async Task<bool> DeleteAsync(int userId, int pso_id)
    {
        var provider = await _context.Providers.FirstOrDefaultAsync(p => p.UserId == userId);
        if (provider == null)
        {
            return false;
        }
        var serviceOffering = await _context.ProviderServices.FirstOrDefaultAsync(pso => pso.Id == pso_id && pso.ProviderId == provider.Id);
        if (serviceOffering == null)
        {
            return false;
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
            return null;
        }
        var providerServiceDtos = providerServices.Select(pso => new ProviderServiceResponseDto
        {
            Id = pso.Id,
            ProviderId = pso.ProviderId,
            Name = pso.Name,
            Description = pso.Description,
            Price = pso.Price,
            DurationInMinutes = pso.DurationMinutes,
            CreatedAt = pso.CreatedAt
        }).ToList();
        return providerServiceDtos;
    }
}
