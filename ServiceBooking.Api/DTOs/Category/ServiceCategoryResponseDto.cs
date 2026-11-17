namespace ServiceBooking.Api.DTOs.Category;

public class ServiceCategoryResponseDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? IconUrl { get; set; }

    public bool IsActive { get; set; }

    public int DisplayOrder { get; set; }

    public int ServiceCount { get; set; }
}
