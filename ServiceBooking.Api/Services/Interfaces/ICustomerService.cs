using ServiceBooking.Api.DTOs.Customers;

namespace ServiceBooking.Api.Services.Interfaces;

public interface ICustomerService
{
    Task<CustomerResponseDto?>
        GetMyProfileAsync(int userId);

    Task<CustomerResponseDto?>
        CreateAsync(int userId);
    
    Task<CustomerResponseDto?>
        UpdateAsync(int userId, CustomerResponseDto dto);
    
    Task<List<CustomerResponseDto>> GetAllAsync();
}