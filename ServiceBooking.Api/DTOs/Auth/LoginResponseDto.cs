namespace ServiceBooking.Api.DTOs.Auth;

public class LoginResponseDto
{

    public string AcessToken { get; set; } = null!;
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Role { get; set; }
}