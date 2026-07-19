using System.ComponentModel.DataAnnotations;
using Lumora.Application.DTOs.Base;

namespace Lumora.Application.DTOs.Housings;

public class HousingCardDto : BaseDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal PricePerMonthUsd { get; set; }
    public string? ImageUrl { get; set; }
    public string CityName { get; set; } = string.Empty;
}

public class HousingDetailsDto : HousingCardDto
{
    public int CityId { get; set; }
    public IReadOnlyList<string> ImageUrls { get; set; } = [];
}

/// Mirrors Domain HousingQueryParameters; bound from the querystring on the public page.
public class HousingFilterDto
{
    public int? CityId { get; set; }
    public decimal? MaxPricePerMonthUsd { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}

public class HousingTranslationInputDto : BaseTranslationDto
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(300)]
    public string Address { get; set; } = string.Empty;
}

public class HousingCreateDto
{
    [Range(0, 100_000)]
    public decimal PricePerMonthUsd { get; set; }

    public string? ImageUrl { get; set; }

    [Required]
    public int CityId { get; set; }

    [Required]
    [MinLength(1)]
    public List<HousingTranslationInputDto> Translations { get; set; } = [];
}

public class HousingUpdateDto : HousingCreateDto
{
}
