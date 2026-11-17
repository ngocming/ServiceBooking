using ServiceBooking.Api.DTOs.Availability;

namespace ServiceBooking.Api.Services.Interfaces;

public interface IAvailabilityService
{
    Task<ProviderScheduleResponseDto> GetProviderScheduleAsync(int providerId);

    Task<ProviderScheduleResponseDto> GetMyScheduleAsync(int userId);

    Task SetWeeklyScheduleAsync(int userId, SetWeeklyAvailabilityDto dto);

    Task<AvailabilityExceptionDto> AddExceptionAsync(int userId, CreateAvailabilityExceptionDto dto);

    Task DeleteExceptionAsync(int userId, int exceptionId);

    Task<bool> IsSlotAvailableAsync(int providerId, DateTime startAt, DateTime endAt);
}
