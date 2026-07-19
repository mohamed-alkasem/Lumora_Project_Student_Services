using Lumora.Domain.Entities.Base;

namespace Lumora.Domain.Interfaces.Repositories;

public interface ITranslatableRepository<TEntity, TTranslation>
    : IGenericRepository<TEntity>
    where TEntity : BaseTranslatableEntity<TTranslation>
    where TTranslation : BaseTranslation
{
    Task<TEntity?> GetByIdWithTranslationsAsync(int id);
    Task<IEnumerable<TEntity>> GetAllWithTranslationsAsync();
    Task<TEntity?> GetByIdWithTranslationAsync(int id, string languageCode);
}
