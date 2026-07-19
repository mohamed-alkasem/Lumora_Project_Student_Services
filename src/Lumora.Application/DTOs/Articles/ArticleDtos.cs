using System.ComponentModel.DataAnnotations;
using Lumora.Application.DTOs.Base;

namespace Lumora.Application.DTOs.Articles;

public class ArticleCardDto : BaseDto
{
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
    public bool IsPublished { get; set; }
}

public class ArticleDetailsDto : ArticleCardDto
{
    public string Body { get; set; } = string.Empty;
}

public class ArticleTranslationInputDto : BaseTranslationDto
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Summary { get; set; } = string.Empty;

    [Required]
    public string Body { get; set; } = string.Empty;
}

public class ArticleCreateDto
{
    public string? CoverImageUrl { get; set; }
    public bool IsPublished { get; set; } = true;

    [Required]
    [MinLength(1)]
    public List<ArticleTranslationInputDto> Translations { get; set; } = [];
}

public class ArticleUpdateDto : ArticleCreateDto
{
}
