namespace ServiceBooking.Api.Models;

public class Booking
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public int ProviderServiceId { get; set; }

    public DateTime BookingDate { get; set; }
    public string PickupLocation { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;

    public string Status { get; set; } = "Pending";// Pending, Confirmed, Completed, Cancelled

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Customer Customer { get; set; } = null!;

    public ProviderService ProviderService { get; set; } = null!;
}