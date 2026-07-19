using Lumora.Application.DTOs.Cities;
using Lumora.Application.DTOs.Housings;
using Lumora.Domain.Common;

namespace Lumora.Web.Models.Housings;

public class HousingIndexViewModel
{
    public HousingFilterDto Filter { get; init; } = new();
    public IReadOnlyList<CityDto> Cities { get; init; } = [];
    public PagedResult<HousingCardDto> Results { get; init; } = new();
}
