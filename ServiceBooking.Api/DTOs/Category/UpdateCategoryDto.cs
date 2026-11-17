using System.ComponentModel.DataAnnotations;

namespace ServiceBooking.Api.DTOs.Category;

public class UpdateCategoryDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(120)]
    public string? Slug { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public string? IconUrl { get; set; }

    public bool IsActive { get; set; } = true;

    public int DisplayOrder { get; set; } = 0;
}
