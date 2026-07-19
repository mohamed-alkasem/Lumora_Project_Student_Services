using Lumora.Domain.Entities.Base;

namespace Lumora.Domain.Entities;

public class ArticleTranslation : BaseTranslation
{
    public int ArticleId { get; set; }
    public Article Article { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;

    /// Long-form content; paragraphs separated by blank lines.
    public string Body { get; set; } = string.Empty;
}
