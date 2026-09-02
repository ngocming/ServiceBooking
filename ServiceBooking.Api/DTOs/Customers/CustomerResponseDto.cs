namespace ServiceBooking.Api.DTOs.Customers;

public class CustomerResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
}