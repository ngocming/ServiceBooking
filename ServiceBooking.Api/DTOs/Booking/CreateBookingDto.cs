namespace ServiceBooking.Api.DTOs.Booking;

public class CreateBookingDto
{
    public int ProviderServiceId { get; set; }

    public DateTime BookingDate { get; set; }

    public string? Note { get; set; }
}