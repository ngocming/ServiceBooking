namespace ServiceBooking.Api.Models;

public class BookingStatusHistory
{
    public int Id { get; set; }

    public int BookingId { get; set; }

    public BookingStatus? PreviousStatus { get; set; }

    public BookingStatus NewStatus { get; set; }

    public int ChangedByUserId { get; set; }

    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

    public string? Reason { get; set; }

    // Navigation properties
    public Booking Booking { get; set; } = null!;

    public User ChangedByUser { get; set; } = null!;
}
