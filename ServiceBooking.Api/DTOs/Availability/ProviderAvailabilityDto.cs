namespace ServiceBooking.Api.DTOs.Availability;

public class ProviderAvailabilityDto
{
    public int Id { get; set; }

    public DayOfWeek DayOfWeek { get; set; }

    public string DayName => DayOfWeek.ToString();

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public bool IsAvailable { get; set; } = true;
}
