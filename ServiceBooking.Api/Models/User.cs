namespace ServiceBooking.Api.Models;

public class User
{
    public int Id { get; set; }

    public required string Username { get; set; }

    public required string Email { get; set; }

    public required string PasswordHash { get; set; }

    public string? FullName { get; set; }

    public string? PhoneNumber { get; set; }
    public string Address { get; set; } = string.Empty;

    public string Role { get; set; } = "User"; // Customer, Provider, Admin

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    
    public Provider? Provider { get; set; }
    public Customer? Customer { get; set; }
}
