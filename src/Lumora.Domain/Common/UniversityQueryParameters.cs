using Lumora.Domain.Enums;

namespace Lumora.Domain.Common;

public class UniversityQueryParameters
{
    public const int MaxPageSize = 50;

    public string? SearchTerm { get; set; }
    public UniversityType? Type { get; set; }
    public int? CityId { get; set; }
    public decimal? MaxAnnualTuitionUsd { get; set; }
    public UniversitySortOption SortBy { get; set; } = UniversitySortOption.TopRated;

    private int _page = 1;
    public int Page
    {
        get => _page;
        set => _page = value < 1 ? 1 : value;
    }

    private int _pageSize = 12;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value < 1 ? 12 : Math.Min(value, MaxPageSize);
    }
}
