using ServiceBooking.Api.DTOs.ProviderService;

namespace ServiceBooking.Api.Services.Interfaces;

public interface IProviderService_OS
{
    Task<List<ProviderServiceResponseDto>?> GetAllAsync();
    Task<List<ProviderServiceResponseDto>> GetByProviderIdAsync(int providerId);
    Task<ProviderServiceResponseDto?> GetByIdAsync(int ps_id);
    Task<ProviderServiceResponseDto?> CreateAsync(CreateProviderServiceDto dto, int userId);
    //Task<ProviderServiceResponseDto?> UpdateAsync(int ps_id, UpdateProviderServiceDto dto);
    Task<bool> DeleteAsync(int userId, int pso_id);
}