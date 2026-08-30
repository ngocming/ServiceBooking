namespace ServiceBooking.Api.DTOs.Providers;

public class ProviderResponseDto
{
    public int Id { get; set; }
    public required int UserId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public bool IsAvailable { get; set; }
    public DateTime CreatedAt { get; set; }
}