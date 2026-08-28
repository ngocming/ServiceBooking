namespace ServiceBooking.Api.Models;

public class Service
{
    public int Id { get; set; }

    public int ProviderId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int DurationMinutes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Provider Provider { get; set; } = null!;
}