namespace ServiceBooking.Api.Models;

public class Booking
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public int ProviderId { get; set; }

    public int ProviderServiceId { get; set; }

    public DateTime BookingDate { get; set; }

    public BookingStatus Status { get; set; } = BookingStatus.Pending;

    public string? Note { get; set; }

    public decimal TotalPrice { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Customer Customer { get; set; } = null!;

    public Provider Provider { get; set; } = null!;

    public ProviderService ProviderService { get; set; } = null!;
}