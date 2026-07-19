using Lumora.Application.DTOs.Articles;
using Lumora.Application.Interfaces.Services;
using Lumora.Application.Services.Base;
using Lumora.Domain.Entities;
using Lumora.Domain.Interfaces.Repositories;

namespace Lumora.Application.Services;

public class ArticleService
    : TranslatableCrudService<Article, ArticleTranslation,
        ArticleDetailsDto, ArticleCreateDto, ArticleUpdateDto>,
      IArticleService
{
    public ArticleService(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }

    public async Task<IReadOnlyList<ArticleCardDto>> GetPublishedAsync(string languageCode)
    {
        var articles = await Repository.GetAllWithTranslationsAsync();
        return articles
            .Where(a => a.IsPublished)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => (ArticleCardDto)MapToDto(a, languageCode))
            .ToList();
    }

    protected override ArticleDetailsDto MapToDto(Article entity, string languageCode)
    {
        var translation = entity.GetTranslationWithFallback(languageCode);
        return new ArticleDetailsDto
        {
            Id = entity.Id,
            CreatedAt = entity.CreatedAt,
            Title = translation?.Title ?? string.Empty,
            Summary = translation?.Summary ?? string.Empty,
            Body = translation?.Body ?? string.Empty,
            CoverImageUrl = entity.CoverImageUrl,
            IsPublished = entity.IsPublished
        };
    }

    protected override Article MapToNewEntity(ArticleCreateDto dto)
    {
        var article = new Article();
        Apply(article, dto);
        return article;
    }

    protected override void ApplyUpdate(Article entity, ArticleUpdateDto dto) =>
        Apply(entity, dto);

    protected override ArticleUpdateDto MapToUpdateDto(Article entity) => new()
    {
        CoverImageUrl = entity.CoverImageUrl,
        IsPublished = entity.IsPublished,
        Translations = entity.Translations
            .Select(t => new ArticleTranslationInputDto
            {
                LanguageCode = t.LanguageCode,
                Title = t.Title,
                Summary = t.Summary,
                Body = t.Body
            })
            .ToList()
    };

    private static void Apply(Article article, ArticleCreateDto dto)
    {
        article.CoverImageUrl = dto.CoverImageUrl;
        article.IsPublished = dto.IsPublished;

        SyncTranslations(
            article.Translations,
            dto.Translations,
            t => t.LanguageCode,
            t => new ArticleTranslation { Title = t.Title, Summary = t.Summary, Body = t.Body },
            (existing, input) =>
            {
                existing.Title = input.Title;
                existing.Summary = input.Summary;
                existing.Body = input.Body;
            });
    }
}
