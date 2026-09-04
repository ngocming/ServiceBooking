using ServiceBooking.Api.DTOs.ProviderService;

namespace ServiceBooking.Api.Services.Interfaces;

public interface IProviderServiceOfferingService
{
    Task<List<ProviderServiceResponseDto>?> GetAllAsync();
    Task<List<ProviderServiceResponseDto>> GetByProviderIdAsync(int providerId);
    Task<ProviderServiceResponseDto?> GetByIdAsync(int id);
    Task<ProviderServiceResponseDto?> CreateAsync(CreateProviderServiceDto dto, int userId);
    Task<bool> DeleteAsync(int userId, int id);
}