namespace ServiceBooking.Api.DTOs.Availability;

public class AvailabilityExceptionDto
{
    public int Id { get; set; }

    public DateOnly Date { get; set; }

    public TimeSpan? StartTime { get; set; }

    public TimeSpan? EndTime { get; set; }

    public bool IsAvailable { get; set; }

    public string? Reason { get; set; }
}
