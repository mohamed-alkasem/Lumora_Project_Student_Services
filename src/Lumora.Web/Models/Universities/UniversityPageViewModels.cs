using Lumora.Application.DTOs.Cities;
using Lumora.Application.DTOs.Universities;
using Lumora.Domain.Common;

namespace Lumora.Web.Models.Universities;

public class UniversityIndexViewModel
{
    public UniversityFilterDto Filter { get; init; } = new();
    public IReadOnlyList<CityDto> Cities { get; init; } = [];
    public UniversityTypeCountsDto TypeCounts { get; init; } = new();
    public UniversityListViewModel List { get; init; } = new();
    public bool IsStudent { get; init; }
}

/// Model of the AJAX-replaceable fragment (grid + pagination).
public class UniversityListViewModel
{
    public PagedResult<UniversityCardDto> Results { get; init; } = new();
    public IReadOnlySet<int> FavoriteIds { get; init; } = new HashSet<int>();
}

public record UniversityCardViewModel(UniversityCardDto Card, bool IsFavorite);
