using Lumora.Application.DTOs.Faqs;
using Lumora.Application.Interfaces.Services;
using Lumora.Application.Services.Base;
using Lumora.Domain.Entities;
using Lumora.Domain.Interfaces.Repositories;

namespace Lumora.Application.Services;

public class FaqService
    : TranslatableCrudService<Faq, FaqTranslation, FaqDto, FaqCreateDto, FaqUpdateDto>,
      IFaqService
{
    public FaqService(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }

    public async Task<IReadOnlyList<FaqDto>> GetOrderedAsync(string languageCode)
    {
        var faqs = await Repository.GetAllWithTranslationsAsync();
        return faqs
            .OrderBy(f => f.DisplayOrder)
            .Select(f => MapToDto(f, languageCode))
            .ToList();
    }

    protected override FaqDto MapToDto(Faq entity, string languageCode)
    {
        var translation = entity.GetTranslationWithFallback(languageCode);
        return new FaqDto
        {
            Id = entity.Id,
            CreatedAt = entity.CreatedAt,
            Question = translation?.Question ?? string.Empty,
            Answer = translation?.Answer ?? string.Empty,
            DisplayOrder = entity.DisplayOrder
        };
    }

    protected override Faq MapToNewEntity(FaqCreateDto dto)
    {
        var faq = new Faq { DisplayOrder = dto.DisplayOrder };
        ApplyTranslations(faq, dto);
        return faq;
    }

    protected override void ApplyUpdate(Faq entity, FaqUpdateDto dto)
    {
        entity.DisplayOrder = dto.DisplayOrder;
        ApplyTranslations(entity, dto);
    }

    protected override FaqUpdateDto MapToUpdateDto(Faq entity) => new()
    {
        DisplayOrder = entity.DisplayOrder,
        Translations = entity.Translations
            .Select(t => new FaqTranslationInputDto
            {
                LanguageCode = t.LanguageCode,
                Question = t.Question,
                Answer = t.Answer
            })
            .ToList()
    };

    private static void ApplyTranslations(Faq faq, FaqCreateDto dto) =>
        SyncTranslations(
            faq.Translations,
            dto.Translations,
            t => t.LanguageCode,
            t => new FaqTranslation { Question = t.Question, Answer = t.Answer },
            (existing, input) =>
            {
                existing.Question = input.Question;
                existing.Answer = input.Answer;
            });
}
