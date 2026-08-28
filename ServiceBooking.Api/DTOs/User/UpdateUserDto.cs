namespace ServiceBooking.Api.DTOs.User;

public class UpdateUserDto
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public string Role { get; set; } = "Customer";
}
