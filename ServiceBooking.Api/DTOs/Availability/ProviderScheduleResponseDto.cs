namespace ServiceBooking.Api.DTOs.Availability;

public class ProviderScheduleResponseDto
{
    public int ProviderId { get; set; }

    public string ProviderName { get; set; } = string.Empty;

    public List<ProviderAvailabilityDto> WeeklySchedule { get; set; } = new();

    public List<AvailabilityExceptionDto> Exceptions { get; set; } = new();
}
