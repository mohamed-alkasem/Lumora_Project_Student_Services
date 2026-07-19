using Lumora.Domain.Entities.Base;

namespace Lumora.Domain.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<T> Repository<T>() where T : BaseEntity;

    ITranslatableRepository<TEntity, TTranslation> TranslatableRepository<TEntity, TTranslation>()
        where TEntity : BaseTranslatableEntity<TTranslation>
        where TTranslation : BaseTranslation;

    Task<int> CommitAsync();
    Task RollbackAsync();
}
