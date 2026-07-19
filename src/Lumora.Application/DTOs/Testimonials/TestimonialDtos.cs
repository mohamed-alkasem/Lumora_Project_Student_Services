using System.ComponentModel.DataAnnotations;
using Lumora.Application.DTOs.Base;

namespace Lumora.Application.DTOs.Testimonials;

public class TestimonialDto : BaseDto
{
    public string StudentName { get; set; } = string.Empty;
    public string StudentTitle { get; set; } = string.Empty;
    public string QuoteText { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsPublished { get; set; }
}

public class TestimonialTranslationInputDto : BaseTranslationDto
{
    [Required]
    [StringLength(150)]
    public string StudentName { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string StudentTitle { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string QuoteText { get; set; } = string.Empty;
}

public class TestimonialCreateDto
{
    public string? PhotoUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsPublished { get; set; } = true;

    [Required]
    [MinLength(1)]
    public List<TestimonialTranslationInputDto> Translations { get; set; } = [];
}

public class TestimonialUpdateDto : TestimonialCreateDto
{
}
