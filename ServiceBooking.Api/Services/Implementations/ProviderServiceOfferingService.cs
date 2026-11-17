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
            .Include(pso => pso.Category)
            .Include(pso => pso.Provider).ThenInclude(p => p.User)
            .Where(pso => pso.ProviderId == providerId)
            .Select(pso => new ProviderServiceResponseDto
            {
                Id = pso.Id,
                ProviderId = pso.ProviderId,
                ProviderName = pso.Provider.DisplayName ?? pso.Provider.User.Username,
                CategoryId = pso.CategoryId,
                CategoryName = pso.Category != null ? pso.Category.Name : null,
                Name = pso.Name,
                Description = pso.Description,
                ImageUrl = pso.ImageUrl,
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
            .Include(pso => pso.Category)
            .Include(pso => pso.Provider).ThenInclude(p => p.User)
            .Where(pso => pso.Id == id)
            .Select(pso => new ProviderServiceResponseDto
            {
                Id = pso.Id,
                ProviderId = pso.ProviderId,
                ProviderName = pso.Provider.DisplayName ?? pso.Provider.User.Username,
                CategoryId = pso.CategoryId,
                CategoryName = pso.Category != null ? pso.Category.Name : null,
                Name = pso.Name,
                Description = pso.Description,
                ImageUrl = pso.ImageUrl,
                Price = pso.Price,
                DurationMinutes = pso.DurationMinutes,
                IsAvailable = pso.IsAvailable,
                CreatedAt = pso.CreatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ProviderServiceResponseDto?> CreateAsync(CreateProviderServiceDto dto, int userId)
    {
        var provider = await _context.Providers
            .FirstOrDefaultAsync(p => p.UserId == userId);
        if (provider == null)
        {
            throw new UnauthorizedAccessException("User is not registered as a provider.");
        }

        var providerService = new Models.ProviderService
        {
            ProviderId = provider.Id,
            CategoryId = dto.CategoryId,
            Name = dto.Name,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
            Price = dto.Price,
            DurationMinutes = dto.DurationMinutes,
            IsAvailable = true
        };

        _context.ProviderServices.Add(providerService);
        await _context.SaveChangesAsync();

        var categoryName = dto.CategoryId.HasValue
            ? await _context.ServiceCategories.Where(c => c.Id == dto.CategoryId).Select(c => c.Name).FirstOrDefaultAsync()
            : null;

        return new ProviderServiceResponseDto
        {
            Id = providerService.Id,
            ProviderId = providerService.ProviderId,
            ProviderName = provider.DisplayName ?? provider.User?.Username,
            CategoryId = providerService.CategoryId,
            CategoryName = categoryName,
            Name = providerService.Name,
            Description = providerService.Description,
            ImageUrl = providerService.ImageUrl,
            Price = providerService.Price,
            DurationMinutes = providerService.DurationMinutes,
            IsAvailable = providerService.IsAvailable,
            CreatedAt = providerService.CreatedAt
        };
    }

    public async Task<ProviderServiceResponseDto?> UpdateAsync(int userId, int id, UpdateProviderServiceDto dto)
    {
        var provider = await _context.Providers
            .FirstOrDefaultAsync(p => p.UserId == userId);
        if (provider == null)
        {
            throw new UnauthorizedAccessException("User is not registered as a provider.");
        }

        var serviceOffering = await _context.ProviderServices
            .Include(pso => pso.Category)
            .FirstOrDefaultAsync(pso => pso.Id == id);
        if (serviceOffering == null)
        {
            throw new KeyNotFoundException("Provider service not found.");
        }

        if (serviceOffering.ProviderId != provider.Id)
        {
            throw new UnauthorizedAccessException("You do not have permission to update this service.");
        }

        serviceOffering.CategoryId = dto.CategoryId;
        serviceOffering.Name = dto.Name;
        serviceOffering.Description = dto.Description;
        serviceOffering.ImageUrl = dto.ImageUrl;
        serviceOffering.Price = dto.Price;
        serviceOffering.DurationMinutes = dto.DurationMinutes;
        serviceOffering.IsAvailable = dto.IsAvailable;

        await _context.SaveChangesAsync();

        var categoryName = serviceOffering.CategoryId.HasValue
            ? await _context.ServiceCategories.Where(c => c.Id == serviceOffering.CategoryId).Select(c => c.Name).FirstOrDefaultAsync()
            : null;

        return new ProviderServiceResponseDto
        {
            Id = serviceOffering.Id,
            ProviderId = serviceOffering.ProviderId,
            ProviderName = provider.DisplayName ?? provider.User?.Username,
            CategoryId = serviceOffering.CategoryId,
            CategoryName = categoryName,
            Name = serviceOffering.Name,
            Description = serviceOffering.Description,
            ImageUrl = serviceOffering.ImageUrl,
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
        var providerServices = await _context.ProviderServices
            .Include(pso => pso.Category)
            .Include(pso => pso.Provider).ThenInclude(p => p.User)
            .ToListAsync();

        if (providerServices == null || !providerServices.Any())
        {
            return new List<ProviderServiceResponseDto>();
        }

        return providerServices.Select(pso => new ProviderServiceResponseDto
        {
            Id = pso.Id,
            ProviderId = pso.ProviderId,
            ProviderName = pso.Provider?.DisplayName ?? pso.Provider?.User?.Username,
            CategoryId = pso.CategoryId,
            CategoryName = pso.Category?.Name,
            Name = pso.Name,
            Description = pso.Description,
            ImageUrl = pso.ImageUrl,
            Price = pso.Price,
            DurationMinutes = pso.DurationMinutes,
            IsAvailable = pso.IsAvailable,
            CreatedAt = pso.CreatedAt
        }).ToList();
    }
}
