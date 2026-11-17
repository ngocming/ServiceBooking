namespace ServiceBooking.Api.DTOs.Availability;

public class WeeklySlotDto
{
    public DayOfWeek DayOfWeek { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public bool IsAvailable { get; set; } = true;
}

public class SetWeeklyAvailabilityDto
{
    public List<WeeklySlotDto> Slots { get; set; } = new();
}
