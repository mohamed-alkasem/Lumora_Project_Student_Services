using System.ComponentModel.DataAnnotations;
using Lumora.Application.DTOs.Base;

namespace Lumora.Application.DTOs.Scholarships;

public class ScholarshipCardDto : BaseDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int UniversityId { get; set; }
    public string UniversityName { get; set; } = string.Empty;
    public decimal OriginalPriceUsd { get; set; }
    public decimal DiscountedPriceUsd { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime? Deadline { get; set; }
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }

    public int DiscountPercent => OriginalPriceUsd > 0
        ? (int)Math.Round(100 - DiscountedPriceUsd * 100 / OriginalPriceUsd)
        : 0;
}

public class ScholarshipTranslationInputDto : BaseTranslationDto
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;
}

public class ScholarshipCreateDto
{
    [Required]
    public int UniversityId { get; set; }

    [Range(1, 1_000_000)]
    public decimal OriginalPriceUsd { get; set; }

    [Range(0, 1_000_000)]
    public decimal DiscountedPriceUsd { get; set; }

    public string? ImageUrl { get; set; }
    public DateTime? Deadline { get; set; }
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }

    [Required]
    [MinLength(1)]
    public List<ScholarshipTranslationInputDto> Translations { get; set; } = [];
}

public class ScholarshipUpdateDto : ScholarshipCreateDto
{
}
