using System.ComponentModel.DataAnnotations;

namespace ServiceBooking.Api.DTOs.Auth;

public class RegisterRequestDto
{
    [Required, StringLength(50)]
    public required string Username { get; init; }
    
    [Required, StringLength(100)]
    [EmailAddress]
    public required string Email { get; init; }
    
    [Required, StringLength(100)]
    [DataType(DataType.Password)]
    public required string Password { get; init; }
 // Customer, Provider, Admin
    
}
