namespace ServiceBooking.Api.Models;

public class Provider
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string DisplayName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public bool IsAvailable { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;

    public ICollection<Service> Services { get; set; }
        = new List<Service>();
}