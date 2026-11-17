namespace ServiceBooking.Api.DTOs.Availability;

public class CreateAvailabilityExceptionDto
{
    public DateOnly Date { get; set; }

    public TimeSpan? StartTime { get; set; }

    public TimeSpan? EndTime { get; set; }

    public bool IsAvailable { get; set; } = false;

    public string? Reason { get; set; }
}
