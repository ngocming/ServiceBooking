using System.ComponentModel.DataAnnotations;

namespace ServiceBooking.Api.DTOs.ProviderService;

public class CreateProviderServiceDto
{
    [Required]
    public required string Name { get; set; }
    
    public required string Description { get; set; }
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
    public required decimal Price { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Duration must be greater than 0.")]
    public required int DurationMinutes { get; set; }
}