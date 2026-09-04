namespace ServiceBooking.Api.DTOs.Booking;

public class BookingResponseDto
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public int ProviderId { get; set; }

    public int ProviderServiceId { get; set; }

    public string ServiceName { get; set; } = string.Empty;

    public string ProviderName { get; set; } = string.Empty;

    public DateTime BookingDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public string? Note { get; set; }

    public decimal TotalPrice { get; set; }

    public DateTime CreatedAt { get; set; }
}