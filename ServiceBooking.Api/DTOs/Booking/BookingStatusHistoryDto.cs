namespace ServiceBooking.Api.DTOs.Booking;

public class BookingStatusHistoryDto
{
    public int Id { get; set; }

    public int BookingId { get; set; }

    public string? PreviousStatus { get; set; }

    public string NewStatus { get; set; } = string.Empty;

    public int ChangedByUserId { get; set; }

    public string ChangedByUsername { get; set; } = string.Empty;

    public DateTime ChangedAt { get; set; }

    public string? Reason { get; set; }
}
