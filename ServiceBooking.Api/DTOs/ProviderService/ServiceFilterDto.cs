namespace ServiceBooking.Api.DTOs.ProviderService;

public class ServiceFilterDto
{
    public string? SearchTerm { get; set; }

    public int? CategoryId { get; set; }

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }

    public int? ProviderId { get; set; }

    public bool? IsAvailableOnly { get; set; } = true;

    public string? SortBy { get; set; }

    public int PageNumber { get; set; } = 1;

    public int? PageIndex
    {
        get => PageNumber;
        set
        {
            if (value.HasValue && value.Value > 0)
                PageNumber = value.Value;
        }
    }

    public int PageSize { get; set; } = 10;
}
