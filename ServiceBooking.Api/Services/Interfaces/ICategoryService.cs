using ServiceBooking.Api.DTOs.Category;

namespace ServiceBooking.Api.Services.Interfaces;

public interface ICategoryService
{
    Task<List<ServiceCategoryResponseDto>> GetAllAsync(bool activeOnly = true);
    Task<ServiceCategoryResponseDto?> GetByIdAsync(int id);
    Task<ServiceCategoryResponseDto?> GetBySlugAsync(string slug);
    Task<ServiceCategoryResponseDto> CreateAsync(CreateCategoryDto dto);
    Task<ServiceCategoryResponseDto> UpdateAsync(int id, UpdateCategoryDto dto);
    Task DeleteAsync(int id);
}
