using Lumora.Application.DTOs.Testimonials;
using Lumora.Application.Interfaces.Services;
using Lumora.Application.Services.Base;
using Lumora.Domain.Entities;
using Lumora.Domain.Interfaces.Repositories;

namespace Lumora.Application.Services;

public class TestimonialService
    : TranslatableCrudService<Testimonial, TestimonialTranslation,
        TestimonialDto, TestimonialCreateDto, TestimonialUpdateDto>,
      ITestimonialService
{
    public TestimonialService(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }

    public async Task<IReadOnlyList<TestimonialDto>> GetPublishedAsync(string languageCode)
    {
        var testimonials = await Repository.GetAllWithTranslationsAsync();
        return testimonials
            .Where(t => t.IsPublished)
            .OrderBy(t => t.DisplayOrder)
            .Select(t => MapToDto(t, languageCode))
            .ToList();
    }

    protected override TestimonialDto MapToDto(Testimonial entity, string languageCode)
    {
        var translation = entity.GetTranslationWithFallback(languageCode);
        return new TestimonialDto
        {
            Id = entity.Id,
            CreatedAt = entity.CreatedAt,
            StudentName = translation?.StudentName ?? string.Empty,
            StudentTitle = translation?.StudentTitle ?? string.Empty,
            QuoteText = translation?.QuoteText ?? string.Empty,
            PhotoUrl = entity.PhotoUrl,
            DisplayOrder = entity.DisplayOrder,
            IsPublished = entity.IsPublished
        };
    }

    protected override Testimonial MapToNewEntity(TestimonialCreateDto dto)
    {
        var testimonial = new Testimonial
        {
            PhotoUrl = dto.PhotoUrl,
            DisplayOrder = dto.DisplayOrder,
            IsPublished = dto.IsPublished
        };
        ApplyTranslations(testimonial, dto);
        return testimonial;
    }

    protected override void ApplyUpdate(Testimonial entity, TestimonialUpdateDto dto)
    {
        entity.PhotoUrl = dto.PhotoUrl;
        entity.DisplayOrder = dto.DisplayOrder;
        entity.IsPublished = dto.IsPublished;
        ApplyTranslations(entity, dto);
    }

    protected override TestimonialUpdateDto MapToUpdateDto(Testimonial entity) => new()
    {
        PhotoUrl = entity.PhotoUrl,
        DisplayOrder = entity.DisplayOrder,
        IsPublished = entity.IsPublished,
        Translations = entity.Translations
            .Select(t => new TestimonialTranslationInputDto
            {
                LanguageCode = t.LanguageCode,
                StudentName = t.StudentName,
                StudentTitle = t.StudentTitle,
                QuoteText = t.QuoteText
            })
            .ToList()
    };

    private static void ApplyTranslations(Testimonial testimonial, TestimonialCreateDto dto) =>
        SyncTranslations(
            testimonial.Translations,
            dto.Translations,
            t => t.LanguageCode,
            t => new TestimonialTranslation
            {
                StudentName = t.StudentName,
                StudentTitle = t.StudentTitle,
                QuoteText = t.QuoteText
            },
            (existing, input) =>
            {
                existing.StudentName = input.StudentName;
                existing.StudentTitle = input.StudentTitle;
                existing.QuoteText = input.QuoteText;
            });
}
