namespace ServiceBooking.Api.Models;

public class AvailabilityException
{
    public int Id { get; set; }

    public int ProviderId { get; set; }

    public DateOnly Date { get; set; }

    public TimeSpan? StartTime { get; set; }

    public TimeSpan? EndTime { get; set; }

    public bool IsAvailable { get; set; } = false;

    public string? Reason { get; set; }

    // Navigation property
    public Provider Provider { get; set; } = null!;
}
