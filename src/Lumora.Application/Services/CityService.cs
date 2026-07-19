using Lumora.Application.DTOs.Cities;
using Lumora.Application.Interfaces.Services;
using Lumora.Application.Services.Base;
using Lumora.Domain.Entities;
using Lumora.Domain.Interfaces.Repositories;

namespace Lumora.Application.Services;

public class CityService
    : TranslatableCrudService<City, CityTranslation, CityDto, CityCreateDto, CityUpdateDto>,
      ICityService
{
    public CityService(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }

    protected override CityDto MapToDto(City entity, string languageCode) => new()
    {
        Id = entity.Id,
        CreatedAt = entity.CreatedAt,
        Name = entity.GetTranslationWithFallback(languageCode)?.Name ?? string.Empty
    };

    protected override City MapToNewEntity(CityCreateDto dto)
    {
        var city = new City();
        ApplyTranslations(city, dto);
        return city;
    }

    protected override void ApplyUpdate(City entity, CityUpdateDto dto) =>
        ApplyTranslations(entity, dto);

    protected override CityUpdateDto MapToUpdateDto(City entity) => new()
    {
        Translations = entity.Translations
            .Select(t => new CityTranslationInputDto { LanguageCode = t.LanguageCode, Name = t.Name })
            .ToList()
    };

    private static void ApplyTranslations(City city, CityCreateDto dto) =>
        SyncTranslations(
            city.Translations,
            dto.Translations,
            t => t.LanguageCode,
            t => new CityTranslation { Name = t.Name },
            (existing, input) => existing.Name = input.Name);
}
