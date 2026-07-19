namespace Lumora.Application.Interfaces.Services;

public interface ITranslatableService<TDto, TCreateDto, TUpdateDto>
    : IBaseService<TDto, TCreateDto, TUpdateDto>
    where TDto : class
    where TCreateDto : class
    where TUpdateDto : class
{
    Task<TDto?> GetByIdInLanguageAsync(int id, string languageCode);
    Task<IEnumerable<TDto>> GetAllInLanguageAsync(string languageCode);

    /// Admin edit form: the entity's scalars plus ALL translations (not a single-language projection).
    Task<TUpdateDto?> GetForEditAsync(int id);
}
