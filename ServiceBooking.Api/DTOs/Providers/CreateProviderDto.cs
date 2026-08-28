namespace ServiceBooking.Api.DTOs.Providers;

public class CreateProviderDto
{
    public required string DisplayName { get; set; }
    public required string Description { get; set; }
    public required string Phone { get; set; }
    public required string Address { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public bool IsAvailable { get; set; }
}