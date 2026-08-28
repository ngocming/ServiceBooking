namespace ServiceBooking.Api.Models;

public class User
{
    public int Id { get; set; }

    public required string Username { get; set; }

    public required string Email { get; set; }

    public required string PasswordHash { get; set; }

    public string? FullName { get; set; }

    public string? PhoneNumber { get; set; }

    public string Role { get; set; } = "Customer"; // Customer, Provider, Admin

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public Provider? Provider { get; set; }
}
