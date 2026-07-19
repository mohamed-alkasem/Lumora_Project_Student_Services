using System.ComponentModel.DataAnnotations;
using Lumora.Application.DTOs.Base;
using Lumora.Domain.Enums;

namespace Lumora.Application.DTOs.Universities;

/// Card shown in the directory grid.
public class UniversityCardDto : BaseDto
{
    public string Name { get; set; } = string.Empty;
    public UniversityType Type { get; set; }
    public decimal Rating { get; set; }
    public decimal AnnualTuitionUsd { get; set; }
    public int CampusCount { get; set; }
    public string? LogoUrl { get; set; }
    public string CityName { get; set; } = string.Empty;
    public IReadOnlyList<string> ProgramNames { get; set; } = [];
}

public class UniversityDetailsDto : UniversityCardDto
{
    public string ShortDescription { get; set; } = string.Empty;
    public int CityId { get; set; }
    public IReadOnlyList<string> ImageUrls { get; set; } = [];
}

/// Sidebar badges: how many universities exist per type.
public class UniversityTypeCountsDto
{
    public int All { get; set; }
    public int Public { get; set; }
    public int Private { get; set; }
}

/// Mirrors Domain UniversityQueryParameters; bound from the querystring on the public page.
public class UniversityFilterDto
{
    public string? SearchTerm { get; set; }
    public UniversityType? Type { get; set; }
    public int? CityId { get; set; }
    public decimal? MaxAnnualTuitionUsd { get; set; }
    public UniversitySortOption SortBy { get; set; } = UniversitySortOption.TopRated;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}

public class UniversityTranslationInputDto : BaseTranslationDto
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string ShortDescription { get; set; } = string.Empty;
}

public class UniversityCreateDto
{
    [Required]
    public UniversityType Type { get; set; }

    [Range(0, 5)]
    public decimal Rating { get; set; }

    [Range(0, 1_000_000)]
    public decimal AnnualTuitionUsd { get; set; }

    [Range(1, 100)]
    public int CampusCount { get; set; } = 1;

    public string? LogoUrl { get; set; }

    [Required]
    public int CityId { get; set; }

    public List<int> ProgramIds { get; set; } = [];

    [Required]
    [MinLength(1)]
    public List<UniversityTranslationInputDto> Translations { get; set; } = [];
}

public class UniversityUpdateDto : UniversityCreateDto
{
}
