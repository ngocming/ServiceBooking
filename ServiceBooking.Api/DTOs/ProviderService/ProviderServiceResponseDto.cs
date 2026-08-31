namespace ServiceBooking.Api.DTOs.ProviderService;

public class ProviderServiceResponseDto
{
    public int Id { get; set; }
    public int ProviderId { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public decimal Price { get; set; }
    public int DurationInMinutes { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsAvailable { get; set; }
}