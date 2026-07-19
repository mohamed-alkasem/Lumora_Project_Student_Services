using Lumora.Domain.Entities.Base;
using Lumora.Domain.Interfaces.Repositories;
using Lumora.Infrastructure.Data;

namespace Lumora.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly Dictionary<Type, object> _repositories = new();

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IGenericRepository<T> Repository<T>() where T : BaseEntity
    {
        var type = typeof(T);
        if (!_repositories.TryGetValue(type, out var repo))
        {
            repo = new GenericRepository<T>(_context);
            _repositories[type] = repo;
        }
        return (IGenericRepository<T>)repo;
    }

    public ITranslatableRepository<TEntity, TTranslation> TranslatableRepository<TEntity, TTranslation>()
        where TEntity : BaseTranslatableEntity<TTranslation>
        where TTranslation : BaseTranslation
    {
        var type = typeof(TEntity);
        if (!_repositories.TryGetValue(type, out var repo))
        {
            repo = new TranslatableRepository<TEntity, TTranslation>(_context);
            _repositories[type] = repo;
        }
        return (ITranslatableRepository<TEntity, TTranslation>)repo;
    }

    public async Task<int> CommitAsync() =>
        await _context.SaveChangesAsync();

    public async Task RollbackAsync() =>
        await _context.Database.RollbackTransactionAsync();

    public void Dispose() =>
        _context.Dispose();
}
