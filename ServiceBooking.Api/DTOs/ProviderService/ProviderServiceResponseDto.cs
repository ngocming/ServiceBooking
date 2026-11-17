namespace ServiceBooking.Api.DTOs.ProviderService;

public class ProviderServiceResponseDto
{
    public int Id { get; set; }
    public int ProviderId { get; set; }
    public string? ProviderName { get; set; }
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; }
    public int DurationMinutes { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsAvailable { get; set; }
}
