namespace ServiceBooking.Api.Models;

public class ProviderAvailability
{
    public int Id { get; set; }

    public int ProviderId { get; set; }

    public DayOfWeek DayOfWeek { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public bool IsAvailable { get; set; } = true;

    // Navigation property
    public Provider Provider { get; set; } = null!;
}
