using Lumora.Application.DTOs.Scholarships;
using Lumora.Application.Interfaces.Services;
using Lumora.Application.Services.Base;
using Lumora.Domain.Entities;
using Lumora.Domain.Interfaces.Repositories;

namespace Lumora.Application.Services;

public class ScholarshipService
    : TranslatableCrudService<Scholarship, ScholarshipTranslation,
        ScholarshipCardDto, ScholarshipCreateDto, ScholarshipUpdateDto>,
      IScholarshipService
{
    public ScholarshipService(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }

    public async Task<IReadOnlyList<ScholarshipCardDto>> GetActiveAsync(string languageCode)
    {
        var scholarships = await Repository.GetAllWithTranslationsAsync();
        var names = await UniversityNamesAsync(languageCode);

        return scholarships
            .Where(s => s.IsActive)
            .OrderBy(s => s.DisplayOrder)
            .ThenByDescending(s => s.CreatedAt)
            .Select(s => Map(s, languageCode, names))
            .ToList();
    }

    public override async Task<IEnumerable<ScholarshipCardDto>> GetAllInLanguageAsync(string languageCode)
    {
        var scholarships = await Repository.GetAllWithTranslationsAsync();
        var names = await UniversityNamesAsync(languageCode);
        return scholarships
            .OrderBy(s => s.DisplayOrder)
            .Select(s => Map(s, languageCode, names))
            .ToList();
    }

    /// Universities are a small lookup set, so one translated fetch feeds all card names.
    private async Task<Dictionary<int, string>> UniversityNamesAsync(string languageCode)
    {
        var universities = await UnitOfWork
            .TranslatableRepository<University, UniversityTranslation>()
            .GetAllWithTranslationsAsync();
        return universities.ToDictionary(
            u => u.Id,
            u => u.GetTranslationWithFallback(languageCode)?.Name ?? string.Empty);
    }

    private static ScholarshipCardDto Map(Scholarship entity, string languageCode, Dictionary<int, string> names)
    {
        var translation = entity.GetTranslationWithFallback(languageCode);
        return new ScholarshipCardDto
        {
            Id = entity.Id,
            CreatedAt = entity.CreatedAt,
            Title = translation?.Title ?? string.Empty,
            Description = translation?.Description ?? string.Empty,
            UniversityId = entity.UniversityId,
            UniversityName = names.GetValueOrDefault(entity.UniversityId, string.Empty),
            OriginalPriceUsd = entity.OriginalPriceUsd,
            DiscountedPriceUsd = entity.DiscountedPriceUsd,
            ImageUrl = entity.ImageUrl,
            Deadline = entity.Deadline,
            IsActive = entity.IsActive,
            DisplayOrder = entity.DisplayOrder
        };
    }

    protected override ScholarshipCardDto MapToDto(Scholarship entity, string languageCode) =>
        Map(entity, languageCode, new Dictionary<int, string>());

    protected override Scholarship MapToNewEntity(ScholarshipCreateDto dto)
    {
        var scholarship = new Scholarship();
        Apply(scholarship, dto);
        return scholarship;
    }

    protected override void ApplyUpdate(Scholarship entity, ScholarshipUpdateDto dto) =>
        Apply(entity, dto);

    protected override ScholarshipUpdateDto MapToUpdateDto(Scholarship entity) => new()
    {
        UniversityId = entity.UniversityId,
        OriginalPriceUsd = entity.OriginalPriceUsd,
        DiscountedPriceUsd = entity.DiscountedPriceUsd,
        ImageUrl = entity.ImageUrl,
        Deadline = entity.Deadline,
        IsActive = entity.IsActive,
        DisplayOrder = entity.DisplayOrder,
        Translations = entity.Translations
            .Select(t => new ScholarshipTranslationInputDto
            {
                LanguageCode = t.LanguageCode,
                Title = t.Title,
                Description = t.Description
            })
            .ToList()
    };

    private static void Apply(Scholarship scholarship, ScholarshipCreateDto dto)
    {
        scholarship.UniversityId = dto.UniversityId;
        scholarship.OriginalPriceUsd = dto.OriginalPriceUsd;
        scholarship.DiscountedPriceUsd = dto.DiscountedPriceUsd;
        scholarship.ImageUrl = dto.ImageUrl;
        scholarship.Deadline = dto.Deadline;
        scholarship.IsActive = dto.IsActive;
        scholarship.DisplayOrder = dto.DisplayOrder;

        SyncTranslations(
            scholarship.Translations,
            dto.Translations,
            t => t.LanguageCode,
            t => new ScholarshipTranslation { Title = t.Title, Description = t.Description },
            (existing, input) =>
            {
                existing.Title = input.Title;
                existing.Description = input.Description;
            });
    }
}
