using Lumora.Domain.Entities.Base;

namespace Lumora.Domain.Entities;

/// Blog/news article published by the admins.
public class Article : BaseTranslatableEntity<ArticleTranslation>
{
    public string? CoverImageUrl { get; set; }
    public bool IsPublished { get; set; } = true;
}
