namespace ServiceBooking.Api.Models;

public class ProviderService
{
    public int Id { get; set; }

    public int ProviderId { get; set; }

    public int? CategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public decimal Price { get; set; }

    public int DurationMinutes { get; set; }

    public bool IsAvailable { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Provider Provider { get; set; } = null!;

    public ServiceCategory? Category { get; set; }
}
