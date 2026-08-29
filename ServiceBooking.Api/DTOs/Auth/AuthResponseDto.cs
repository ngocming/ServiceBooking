namespace ServiceBooking.Api.DTOs.Auth;

public class AuthResponseDto
{
    public int Id { get; init; }
    public required string Username { get; init; }
    public required string Email { get; init; }
    public required string Role { get; init; }
    public string? Token { get; init; }
}
