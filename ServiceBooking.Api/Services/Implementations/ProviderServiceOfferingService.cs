using ServiceBooking.Api.Data;
using ServiceBooking.Api.DTOs.Common;
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

    public async Task<PagedResult<ProviderServiceResponseDto>> GetFilteredAsync(ServiceFilterDto filter)
    {
        var query = _context.ProviderServices
            .Include(pso => pso.Category)
            .Include(pso => pso.Provider)
            .AsQueryable();

        if (filter.IsAvailableOnly == true)
        {
            query = query.Where(s => s.IsAvailable);
        }

        if (filter.CategoryId.HasValue)
        {
            query = query.Where(s => s.CategoryId == filter.CategoryId.Value);
        }

        if (filter.ProviderId.HasValue)
        {
            query = query.Where(s => s.ProviderId == filter.ProviderId.Value);
        }

        if (filter.MinPrice.HasValue)
        {
            query = query.Where(s => s.Price >= filter.MinPrice.Value);
        }

        if (filter.MaxPrice.HasValue)
        {
            query = query.Where(s => s.Price <= filter.MaxPrice.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = filter.SearchTerm.Trim();
            query = query.Where(s =>
                s.Name.Contains(term) ||
                s.Description.Contains(term) ||
                (s.Provider.DisplayName != null && s.Provider.DisplayName.Contains(term)) ||
                (s.Category != null && s.Category.Name.Contains(term)));
        }

        query = filter.SortBy?.ToLowerInvariant() switch
        {
            "price_asc" => query.OrderBy(s => s.Price),
            "price_desc" => query.OrderByDescending(s => s.Price),
            "name_asc" => query.OrderBy(s => s.Name),
            "duration_asc" => query.OrderBy(s => s.DurationMinutes),
            _ => query.OrderByDescending(s => s.CreatedAt)
        };

        var pageNumber = filter.PageNumber > 0 ? filter.PageNumber : 1;
        var pageSize = filter.PageSize > 0 ? (filter.PageSize > 50 ? 50 : filter.PageSize) : 10;

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(pso => new ProviderServiceResponseDto
            {
                Id = pso.Id,
                ProviderId = pso.ProviderId,
                ProviderName = pso.Provider != null ? (pso.Provider.DisplayName ?? (pso.Provider.User != null ? pso.Provider.User.Username : string.Empty)) : string.Empty,
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

        return new PagedResult<ProviderServiceResponseDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
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
                ProviderName = pso.Provider != null ? (pso.Provider.DisplayName ?? (pso.Provider.User != null ? pso.Provider.User.Username : string.Empty)) : string.Empty,
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
                ProviderName = pso.Provider != null ? (pso.Provider.DisplayName ?? (pso.Provider.User != null ? pso.Provider.User.Username : string.Empty)) : string.Empty,
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
