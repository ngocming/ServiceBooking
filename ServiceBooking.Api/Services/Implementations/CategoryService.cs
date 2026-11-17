using Microsoft.EntityFrameworkCore;
using ServiceBooking.Api.Data;
using ServiceBooking.Api.DTOs.Category;
using ServiceBooking.Api.Models;
using ServiceBooking.Api.Services.Interfaces;
using System.Text.RegularExpressions;

namespace ServiceBooking.Api.Services.Implementations;

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _context;

    public CategoryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ServiceCategoryResponseDto>> GetAllAsync(bool activeOnly = true)
    {
        var query = _context.ServiceCategories
            .Include(c => c.Services)
            .AsQueryable();

        if (activeOnly)
        {
            query = query.Where(c => c.IsActive);
        }

        return await query
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .Select(c => new ServiceCategoryResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                Description = c.Description,
                IconUrl = c.IconUrl,
                IsActive = c.IsActive,
                DisplayOrder = c.DisplayOrder,
                ServiceCount = c.Services.Count(s => s.IsAvailable)
            })
            .ToListAsync();
    }

    public async Task<ServiceCategoryResponseDto?> GetByIdAsync(int id)
    {
        var category = await _context.ServiceCategories
            .Include(c => c.Services)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
            return null;

        return new ServiceCategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            Description = category.Description,
            IconUrl = category.IconUrl,
            IsActive = category.IsActive,
            DisplayOrder = category.DisplayOrder,
            ServiceCount = category.Services.Count(s => s.IsAvailable)
        };
    }

    public async Task<ServiceCategoryResponseDto?> GetBySlugAsync(string slug)
    {
        var category = await _context.ServiceCategories
            .Include(c => c.Services)
            .FirstOrDefaultAsync(c => c.Slug == slug);

        if (category == null)
            return null;

        return new ServiceCategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            Description = category.Description,
            IconUrl = category.IconUrl,
            IsActive = category.IsActive,
            DisplayOrder = category.DisplayOrder,
            ServiceCount = category.Services.Count(s => s.IsAvailable)
        };
    }

    public async Task<ServiceCategoryResponseDto> CreateAsync(CreateCategoryDto dto)
    {
        var slug = string.IsNullOrWhiteSpace(dto.Slug)
            ? GenerateSlug(dto.Name)
            : dto.Slug.Trim().ToLower();

        var existingSlug = await _context.ServiceCategories.AnyAsync(c => c.Slug == slug);
        if (existingSlug)
        {
            slug = $"{slug}-{DateTime.UtcNow.Ticks % 10000}";
        }

        var category = new ServiceCategory
        {
            Name = dto.Name.Trim(),
            Slug = slug,
            Description = dto.Description,
            IconUrl = dto.IconUrl,
            DisplayOrder = dto.DisplayOrder,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.ServiceCategories.Add(category);
        await _context.SaveChangesAsync();

        return new ServiceCategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            Description = category.Description,
            IconUrl = category.IconUrl,
            IsActive = category.IsActive,
            DisplayOrder = category.DisplayOrder,
            ServiceCount = 0
        };
    }

    public async Task<ServiceCategoryResponseDto> UpdateAsync(int id, UpdateCategoryDto dto)
    {
        var category = await _context.ServiceCategories
            .Include(c => c.Services)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
        {
            throw new KeyNotFoundException("Category not found.");
        }

        var slug = string.IsNullOrWhiteSpace(dto.Slug)
            ? GenerateSlug(dto.Name)
            : dto.Slug.Trim().ToLower();

        var slugExists = await _context.ServiceCategories.AnyAsync(c => c.Slug == slug && c.Id != id);
        if (slugExists)
        {
            throw new InvalidOperationException($"Category with slug '{slug}' already exists.");
        }

        category.Name = dto.Name.Trim();
        category.Slug = slug;
        category.Description = dto.Description;
        category.IconUrl = dto.IconUrl;
        category.IsActive = dto.IsActive;
        category.DisplayOrder = dto.DisplayOrder;

        await _context.SaveChangesAsync();

        return new ServiceCategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            Description = category.Description,
            IconUrl = category.IconUrl,
            IsActive = category.IsActive,
            DisplayOrder = category.DisplayOrder,
            ServiceCount = category.Services.Count(s => s.IsAvailable)
        };
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _context.ServiceCategories.FirstOrDefaultAsync(c => c.Id == id);
        if (category == null)
        {
            throw new KeyNotFoundException("Category not found.");
        }

        _context.ServiceCategories.Remove(category);
        await _context.SaveChangesAsync();
    }

    private static string GenerateSlug(string text)
    {
        var slug = text.ToLowerInvariant().Trim();
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-").Trim('-');
        return string.IsNullOrEmpty(slug) ? "category" : slug;
    }
}
