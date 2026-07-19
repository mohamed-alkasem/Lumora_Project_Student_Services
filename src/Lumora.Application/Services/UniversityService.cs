using Lumora.Application.DTOs.Universities;
using Lumora.Application.Interfaces.Services;
using Lumora.Application.Mapping;
using Lumora.Application.Services.Base;
using Lumora.Domain.Common;
using Lumora.Domain.Constants;
using Lumora.Domain.Entities;
using Lumora.Domain.Enums;
using Lumora.Domain.Interfaces.Repositories;

namespace Lumora.Application.Services;

public class UniversityService
    : TranslatableCrudService<University, UniversityTranslation,
        UniversityDetailsDto, UniversityCreateDto, UniversityUpdateDto>,
      IUniversityService
{
    private readonly IUniversityRepository _universities;

    public UniversityService(IUnitOfWork unitOfWork, IUniversityRepository universities)
        : base(unitOfWork)
    {
        _universities = universities;
    }

    public async Task<PagedResult<UniversityCardDto>> SearchAsync(UniversityFilterDto filter, string languageCode)
    {
        var result = await _universities.SearchAsync(new UniversityQueryParameters
        {
            SearchTerm = filter.SearchTerm,
            Type = filter.Type,
            CityId = filter.CityId,
            MaxAnnualTuitionUsd = filter.MaxAnnualTuitionUsd,
            SortBy = filter.SortBy,
            Page = filter.Page,
            PageSize = filter.PageSize
        });

        return result.Map(u => u.ToCardDto(languageCode));
    }

    public async Task<UniversityTypeCountsDto> GetTypeCountsAsync()
    {
        var repository = UnitOfWork.Repository<University>();
        return new UniversityTypeCountsDto
        {
            All = await repository.CountAsync(),
            Public = await repository.CountAsync(u => u.Type == UniversityType.Public),
            Private = await repository.CountAsync(u => u.Type == UniversityType.Private)
        };
    }

    public override async Task<UniversityDetailsDto?> GetByIdInLanguageAsync(int id, string languageCode)
    {
        var university = await _universities.GetWithDetailsAsync(id);
        return university?.ToDetailsDto(languageCode);
    }

    public override async Task<UniversityDetailsDto> CreateAsync(UniversityCreateDto createDto)
    {
        var university = MapToNewEntity(createDto);
        await SyncProgramsAsync(university, createDto.ProgramIds);
        await _universities.AddAsync(university);
        await UnitOfWork.CommitAsync();
        return university.ToDetailsDto(LanguageConstants.Default);
    }

    public override async Task<UniversityDetailsDto?> UpdateAsync(int id, UniversityUpdateDto updateDto)
    {
        var university = await _universities.GetWithDetailsAsync(id);
        if (university is null)
        {
            return null;
        }

        ApplyUpdate(university, updateDto);
        await SyncProgramsAsync(university, updateDto.ProgramIds);
        _universities.Update(university);
        await UnitOfWork.CommitAsync();
        return university.ToDetailsDto(LanguageConstants.Default);
    }

    // Base implementation loads translations only; the edit form also needs ProgramIds.
    public override async Task<UniversityUpdateDto?> GetForEditAsync(int id)
    {
        var university = await _universities.GetWithDetailsAsync(id);
        return university is null ? null : MapToUpdateDto(university);
    }

    protected override UniversityUpdateDto MapToUpdateDto(University entity) => new()
    {
        Type = entity.Type,
        Rating = entity.Rating,
        AnnualTuitionUsd = entity.AnnualTuitionUsd,
        CampusCount = entity.CampusCount,
        LogoUrl = entity.LogoUrl,
        CityId = entity.CityId,
        ProgramIds = entity.Programs.Select(p => p.Id).ToList(),
        Translations = entity.Translations
            .Select(t => new UniversityTranslationInputDto
            {
                LanguageCode = t.LanguageCode,
                Name = t.Name,
                ShortDescription = t.ShortDescription
            })
            .ToList()
    };

    public async Task<IReadOnlyList<Lumora.Application.DTOs.Base.GalleryImageDto>> GetImagesAsync(int universityId)
    {
        var images = await UnitOfWork.Repository<UniversityImage>()
            .FindAsync(i => i.UniversityId == universityId);
        return images
            .OrderBy(i => i.DisplayOrder)
            .Select(i => new Lumora.Application.DTOs.Base.GalleryImageDto { Id = i.Id, Url = i.Url })
            .ToList();
    }

    public async Task AddImageAsync(int universityId, string url)
    {
        var repository = UnitOfWork.Repository<UniversityImage>();
        var existing = await repository.FindAsync(i => i.UniversityId == universityId);
        await repository.AddAsync(new UniversityImage
        {
            UniversityId = universityId,
            Url = url,
            DisplayOrder = existing.Count() + 1
        });
        await UnitOfWork.CommitAsync();
    }

    public async Task<string?> RemoveImageAsync(int imageId)
    {
        var repository = UnitOfWork.Repository<UniversityImage>();
        var image = await repository.GetByIdAsync(imageId);
        if (image is null)
        {
            return null;
        }

        repository.Delete(image);
        await UnitOfWork.CommitAsync();
        return image.Url;
    }

    protected override UniversityDetailsDto MapToDto(University entity, string languageCode) =>
        entity.ToDetailsDto(languageCode);

    protected override University MapToNewEntity(UniversityCreateDto dto)
    {
        var university = new University();
        ApplyScalarsAndTranslations(university, dto);
        return university;
    }

    protected override void ApplyUpdate(University entity, UniversityUpdateDto dto) =>
        ApplyScalarsAndTranslations(entity, dto);

    private static void ApplyScalarsAndTranslations(University university, UniversityCreateDto dto)
    {
        university.Type = dto.Type;
        university.Rating = dto.Rating;
        university.AnnualTuitionUsd = dto.AnnualTuitionUsd;
        university.CampusCount = dto.CampusCount;
        university.LogoUrl = dto.LogoUrl;
        university.CityId = dto.CityId;

        SyncTranslations(
            university.Translations,
            dto.Translations,
            t => t.LanguageCode,
            t => new UniversityTranslation { Name = t.Name, ShortDescription = t.ShortDescription },
            (existing, input) =>
            {
                existing.Name = input.Name;
                existing.ShortDescription = input.ShortDescription;
            });
    }

    private async Task SyncProgramsAsync(University university, IReadOnlyCollection<int> programIds)
    {
        var programs = programIds.Count == 0
            ? []
            : await UnitOfWork.Repository<AcademicProgram>()
                .FindAsync(p => programIds.Contains(p.Id));

        university.Programs.Clear();
        foreach (var program in programs)
        {
            university.Programs.Add(program);
        }
    }
}
