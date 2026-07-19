using Lumora.Application.Interfaces.Services;
using Lumora.Domain.Constants;
using Lumora.Domain.Entities.Base;
using Lumora.Domain.Interfaces.Repositories;

namespace Lumora.Application.Services.Base;

/// Shared CRUD skeleton for translatable entities; concrete services supply the mapping.
public abstract class TranslatableCrudService<TEntity, TTranslation, TDto, TCreateDto, TUpdateDto>
    : ITranslatableService<TDto, TCreateDto, TUpdateDto>
    where TEntity : BaseTranslatableEntity<TTranslation>
    where TTranslation : BaseTranslation
    where TDto : class
    where TCreateDto : class
    where TUpdateDto : class
{
    protected readonly IUnitOfWork UnitOfWork;

    protected TranslatableCrudService(IUnitOfWork unitOfWork)
    {
        UnitOfWork = unitOfWork;
    }

    protected ITranslatableRepository<TEntity, TTranslation> Repository =>
        UnitOfWork.TranslatableRepository<TEntity, TTranslation>();

    protected abstract TDto MapToDto(TEntity entity, string languageCode);
    protected abstract TEntity MapToNewEntity(TCreateDto dto);
    protected abstract void ApplyUpdate(TEntity entity, TUpdateDto dto);
    protected abstract TUpdateDto MapToUpdateDto(TEntity entity);

    public virtual Task<TDto?> GetByIdAsync(int id) =>
        GetByIdInLanguageAsync(id, LanguageConstants.Default);

    public virtual Task<IEnumerable<TDto>> GetAllAsync() =>
        GetAllInLanguageAsync(LanguageConstants.Default);

    public virtual async Task<TDto?> GetByIdInLanguageAsync(int id, string languageCode)
    {
        var entity = await Repository.GetByIdWithTranslationsAsync(id);
        return entity is null ? null : MapToDto(entity, languageCode);
    }

    public virtual async Task<IEnumerable<TDto>> GetAllInLanguageAsync(string languageCode)
    {
        var entities = await Repository.GetAllWithTranslationsAsync();
        return entities.Select(e => MapToDto(e, languageCode)).ToList();
    }

    public virtual async Task<TDto> CreateAsync(TCreateDto createDto)
    {
        var entity = MapToNewEntity(createDto);
        await Repository.AddAsync(entity);
        await UnitOfWork.CommitAsync();
        return MapToDto(entity, LanguageConstants.Default);
    }

    public virtual async Task<TDto?> UpdateAsync(int id, TUpdateDto updateDto)
    {
        var entity = await Repository.GetByIdWithTranslationsAsync(id);
        if (entity is null)
        {
            return null;
        }

        ApplyUpdate(entity, updateDto);
        Repository.Update(entity);
        await UnitOfWork.CommitAsync();
        return MapToDto(entity, LanguageConstants.Default);
    }

    public virtual async Task<TUpdateDto?> GetForEditAsync(int id)
    {
        var entity = await Repository.GetByIdWithTranslationsAsync(id);
        return entity is null ? null : MapToUpdateDto(entity);
    }

    public virtual async Task<bool> DeleteAsync(int id)
    {
        var entity = await Repository.GetByIdAsync(id);
        if (entity is null)
        {
            return false;
        }

        Repository.Delete(entity);
        await UnitOfWork.CommitAsync();
        return true;
    }

    /// Syncs the entity's translation set with admin input: updates rows for languages
    /// that already exist, adds rows for new languages, removes rows the input no longer contains.
    protected static void SyncTranslations<TInput>(
        ICollection<TTranslation> current,
        IEnumerable<TInput> inputs,
        Func<TInput, string> languageOf,
        Func<TInput, TTranslation> create,
        Action<TTranslation, TInput> apply)
    {
        var inputList = inputs.ToList();
        var inputLanguages = inputList.Select(languageOf).ToHashSet();

        foreach (var stale in current.Where(t => !inputLanguages.Contains(t.LanguageCode)).ToList())
        {
            current.Remove(stale);
        }

        foreach (var input in inputList)
        {
            var existing = current.FirstOrDefault(t => t.LanguageCode == languageOf(input));
            if (existing is null)
            {
                var translation = create(input);
                translation.LanguageCode = languageOf(input);
                current.Add(translation);
            }
            else
            {
                apply(existing, input);
            }
        }
    }
}
