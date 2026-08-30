using ServiceBooking.Api.DTOs.Providers;

namespace ServiceBooking.Api.Services.Interfaces;

public interface IProviderService
{
    Task<List<ProviderResponseDto>> GetAllAsync();

    Task<ProviderResponseDto?> GetByIdAsync(int id);

    Task<ProviderResponseDto?> CreateAsync(CreateProviderDto dto, int userId);

    Task<ProviderResponseDto?> UpdateAsync(
        int id,
        UpdateProviderDto dto);

    Task<bool> DeleteAsync(int id);
}
