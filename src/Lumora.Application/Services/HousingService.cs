using Lumora.Application.DTOs.Housings;
using Lumora.Application.Interfaces.Services;
using Lumora.Application.Mapping;
using Lumora.Application.Services.Base;
using Lumora.Domain.Common;
using Lumora.Domain.Entities;
using Lumora.Domain.Interfaces.Repositories;

namespace Lumora.Application.Services;

public class HousingService
    : TranslatableCrudService<Housing, HousingTranslation,
        HousingDetailsDto, HousingCreateDto, HousingUpdateDto>,
      IHousingService
{
    private readonly IHousingRepository _housings;

    public HousingService(IUnitOfWork unitOfWork, IHousingRepository housings)
        : base(unitOfWork)
    {
        _housings = housings;
    }

    public async Task<PagedResult<HousingCardDto>> SearchAsync(HousingFilterDto filter, string languageCode)
    {
        var result = await _housings.SearchAsync(new HousingQueryParameters
        {
            CityId = filter.CityId,
            MaxPricePerMonthUsd = filter.MaxPricePerMonthUsd,
            Page = filter.Page,
            PageSize = filter.PageSize
        });

        return result.Map(h => h.ToCardDto(languageCode));
    }

    public override async Task<HousingDetailsDto?> GetByIdInLanguageAsync(int id, string languageCode)
    {
        var housing = await _housings.GetWithDetailsAsync(id);
        return housing?.ToDetailsDto(languageCode);
    }

    public async Task<IReadOnlyList<Lumora.Application.DTOs.Base.GalleryImageDto>> GetImagesAsync(int housingId)
    {
        var images = await UnitOfWork.Repository<HousingImage>()
            .FindAsync(i => i.HousingId == housingId);
        return images
            .OrderBy(i => i.DisplayOrder)
            .Select(i => new Lumora.Application.DTOs.Base.GalleryImageDto { Id = i.Id, Url = i.Url })
            .ToList();
    }

    public async Task AddImageAsync(int housingId, string url)
    {
        var repository = UnitOfWork.Repository<HousingImage>();
        var existing = await repository.FindAsync(i => i.HousingId == housingId);
        await repository.AddAsync(new HousingImage
        {
            HousingId = housingId,
            Url = url,
            DisplayOrder = existing.Count() + 1
        });
        await UnitOfWork.CommitAsync();
    }

    public async Task<string?> RemoveImageAsync(int imageId)
    {
        var repository = UnitOfWork.Repository<HousingImage>();
        var image = await repository.GetByIdAsync(imageId);
        if (image is null)
        {
            return null;
        }

        repository.Delete(image);
        await UnitOfWork.CommitAsync();
        return image.Url;
    }

    protected override HousingDetailsDto MapToDto(Housing entity, string languageCode) =>
        entity.ToDetailsDto(languageCode);

    protected override Housing MapToNewEntity(HousingCreateDto dto)
    {
        var housing = new Housing();
        ApplyScalarsAndTranslations(housing, dto);
        return housing;
    }

    protected override void ApplyUpdate(Housing entity, HousingUpdateDto dto) =>
        ApplyScalarsAndTranslations(entity, dto);

    protected override HousingUpdateDto MapToUpdateDto(Housing entity) => new()
    {
        PricePerMonthUsd = entity.PricePerMonthUsd,
        ImageUrl = entity.ImageUrl,
        CityId = entity.CityId,
        Translations = entity.Translations
            .Select(t => new HousingTranslationInputDto
            {
                LanguageCode = t.LanguageCode,
                Title = t.Title,
                Description = t.Description,
                Address = t.Address
            })
            .ToList()
    };

    private static void ApplyScalarsAndTranslations(Housing housing, HousingCreateDto dto)
    {
        housing.PricePerMonthUsd = dto.PricePerMonthUsd;
        housing.ImageUrl = dto.ImageUrl;
        housing.CityId = dto.CityId;

        SyncTranslations(
            housing.Translations,
            dto.Translations,
            t => t.LanguageCode,
            t => new HousingTranslation
            {
                Title = t.Title,
                Description = t.Description,
                Address = t.Address
            },
            (existing, input) =>
            {
                existing.Title = input.Title;
                existing.Description = input.Description;
                existing.Address = input.Address;
            });
    }
}
