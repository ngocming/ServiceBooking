namespace ServiceBooking.Api.DTOs.ProviderService;

public class UpdateProviderServiceDto
{
    public int? CategoryId { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; }
    public int DurationMinutes { get; set; }
    public bool IsAvailable { get; set; }
}
