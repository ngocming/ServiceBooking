using ServiceBooking.Api.DTOs.Availability;
using ServiceBooking.Api.DTOs.ProviderService;

namespace ServiceBooking.Api.DTOs.Providers;

public class ProviderPublicProfileDto
{
    public int Id { get; set; }

    public string DisplayName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public bool IsAvailable { get; set; }

    public int CompletedBookingsCount { get; set; }

    public List<ProviderServiceResponseDto> Services { get; set; } = new();

    public List<ProviderAvailabilityDto> WeeklySchedule { get; set; } = new();
}
