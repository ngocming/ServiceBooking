namespace ServiceBooking.Api.Models;

public class Customer
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public string Address { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public User User { get; set; } = null!;
    
}