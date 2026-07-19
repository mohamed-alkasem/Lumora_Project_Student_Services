using Lumora.Domain.Entities.Base;
using Lumora.Domain.Interfaces.Repositories;
using Lumora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lumora.Infrastructure.Repositories;

public class TranslatableRepository<TEntity, TTranslation>
    : GenericRepository<TEntity>,
      ITranslatableRepository<TEntity, TTranslation>
    where TEntity : BaseTranslatableEntity<TTranslation>
    where TTranslation : BaseTranslation
{
    public TranslatableRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<TEntity?> GetByIdWithTranslationsAsync(int id) =>
        await _dbSet
            .Include(e => e.Translations)
            .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

    public async Task<IEnumerable<TEntity>> GetAllWithTranslationsAsync() =>
        await _dbSet
            .Include(e => e.Translations)
            .Where(e => !e.IsDeleted)
            .ToListAsync();

    public async Task<TEntity?> GetByIdWithTranslationAsync(int id, string languageCode) =>
        await _dbSet
            .Include(e => e.Translations.Where(t => t.LanguageCode == languageCode))
            .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
}
