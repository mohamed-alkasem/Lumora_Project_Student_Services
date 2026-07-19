using Lumora.Application.DTOs.Housings;
using Lumora.Application.DTOs.Universities;
using Lumora.Domain.Entities;

namespace Lumora.Application.Mapping;

/// Shared entity→DTO projections used by more than one service.
public static class EntityMappings
{
    public static UniversityCardDto ToCardDto(this University university, string languageCode)
    {
        var dto = new UniversityCardDto();
        university.FillCard(dto, languageCode);
        return dto;
    }

    public static UniversityDetailsDto ToDetailsDto(this University university, string languageCode)
    {
        var dto = new UniversityDetailsDto
        {
            ShortDescription = university.GetTranslationWithFallback(languageCode)?.ShortDescription ?? string.Empty,
            CityId = university.CityId,
            ImageUrls = university.Images
                .Where(i => !i.IsDeleted)
                .OrderBy(i => i.DisplayOrder)
                .Select(i => i.Url)
                .ToList()
        };
        university.FillCard(dto, languageCode);
        return dto;
    }

    private static void FillCard(this University university, UniversityCardDto dto, string languageCode)
    {
        dto.Id = university.Id;
        dto.CreatedAt = university.CreatedAt;
        dto.Name = university.GetTranslationWithFallback(languageCode)?.Name ?? string.Empty;
        dto.Type = university.Type;
        dto.Rating = university.Rating;
        dto.AnnualTuitionUsd = university.AnnualTuitionUsd;
        dto.CampusCount = university.CampusCount;
        dto.LogoUrl = university.LogoUrl;
        dto.CityName = university.City?.GetTranslationWithFallback(languageCode)?.Name ?? string.Empty;
        dto.ProgramNames = university.Programs
            .Select(p => p.GetTranslationWithFallback(languageCode)?.Name ?? string.Empty)
            .Where(n => n.Length > 0)
            .ToList();
    }

    public static HousingCardDto ToCardDto(this Housing housing, string languageCode)
    {
        var dto = new HousingCardDto();
        housing.FillCard(dto, languageCode);
        return dto;
    }

    public static HousingDetailsDto ToDetailsDto(this Housing housing, string languageCode)
    {
        var dto = new HousingDetailsDto
        {
            CityId = housing.CityId,
            ImageUrls = housing.Images
                .Where(i => !i.IsDeleted)
                .OrderBy(i => i.DisplayOrder)
                .Select(i => i.Url)
                .ToList()
        };
        housing.FillCard(dto, languageCode);
        return dto;
    }

    private static void FillCard(this Housing housing, HousingCardDto dto, string languageCode)
    {
        var translation = housing.GetTranslationWithFallback(languageCode);
        dto.Id = housing.Id;
        dto.CreatedAt = housing.CreatedAt;
        dto.Title = translation?.Title ?? string.Empty;
        dto.Description = translation?.Description ?? string.Empty;
        dto.Address = translation?.Address ?? string.Empty;
        dto.PricePerMonthUsd = housing.PricePerMonthUsd;
        dto.ImageUrl = housing.ImageUrl;
        dto.CityName = housing.City?.GetTranslationWithFallback(languageCode)?.Name ?? string.Empty;
    }
}
