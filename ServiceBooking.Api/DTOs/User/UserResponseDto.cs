namespace ServiceBooking.Api.DTOs.User;

public class UserResponseDto
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public required string Role { get; set; }
    public DateTime CreatedAt { get; set; }
}
