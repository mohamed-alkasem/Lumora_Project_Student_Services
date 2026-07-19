using System.ComponentModel.DataAnnotations;
using Lumora.Application.DTOs.Base;

namespace Lumora.Application.DTOs.Faqs;

public class FaqDto : BaseDto
{
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}

public class FaqTranslationInputDto : BaseTranslationDto
{
    [Required]
    [StringLength(300)]
    public string Question { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Answer { get; set; } = string.Empty;
}

public class FaqCreateDto
{
    public int DisplayOrder { get; set; }

    [Required]
    [MinLength(1)]
    public List<FaqTranslationInputDto> Translations { get; set; } = [];
}

public class FaqUpdateDto : FaqCreateDto
{
}
