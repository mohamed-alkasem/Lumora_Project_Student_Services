using Lumora.Application.DTOs.Programs;
using Lumora.Application.Interfaces.Services;
using Lumora.Application.Services.Base;
using Lumora.Domain.Entities;
using Lumora.Domain.Interfaces.Repositories;

namespace Lumora.Application.Services;

public class AcademicProgramService
    : TranslatableCrudService<AcademicProgram, AcademicProgramTranslation,
        AcademicProgramDto, AcademicProgramCreateDto, AcademicProgramUpdateDto>,
      IAcademicProgramService
{
    public AcademicProgramService(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }

    protected override AcademicProgramDto MapToDto(AcademicProgram entity, string languageCode) => new()
    {
        Id = entity.Id,
        CreatedAt = entity.CreatedAt,
        Name = entity.GetTranslationWithFallback(languageCode)?.Name ?? string.Empty
    };

    protected override AcademicProgram MapToNewEntity(AcademicProgramCreateDto dto)
    {
        var program = new AcademicProgram();
        ApplyTranslations(program, dto);
        return program;
    }

    protected override void ApplyUpdate(AcademicProgram entity, AcademicProgramUpdateDto dto) =>
        ApplyTranslations(entity, dto);

    protected override AcademicProgramUpdateDto MapToUpdateDto(AcademicProgram entity) => new()
    {
        Translations = entity.Translations
            .Select(t => new AcademicProgramTranslationInputDto { LanguageCode = t.LanguageCode, Name = t.Name })
            .ToList()
    };

    private static void ApplyTranslations(AcademicProgram program, AcademicProgramCreateDto dto) =>
        SyncTranslations(
            program.Translations,
            dto.Translations,
            t => t.LanguageCode,
            t => new AcademicProgramTranslation { Name = t.Name },
            (existing, input) => existing.Name = input.Name);
}
