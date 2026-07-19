using System.Linq.Expressions;
using Lumora.Domain.Entities.Base;
using Lumora.Domain.Interfaces.Repositories;
using Lumora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lumora.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id) =>
        await _dbSet.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

    public async Task<IEnumerable<T>> GetAllAsync() =>
        await _dbSet.Where(e => !e.IsDeleted).ToListAsync();

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) =>
        await _dbSet.Where(predicate).Where(e => !e.IsDeleted).ToListAsync();

    public async Task AddAsync(T entity) =>
        await _dbSet.AddAsync(entity);

    public void Update(T entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
    }

    public async Task<bool> ExistsAsync(int id) =>
        await _dbSet.AnyAsync(e => e.Id == id && !e.IsDeleted);

    public async Task<int> CountAsync() =>
        await _dbSet.CountAsync(e => !e.IsDeleted);

    public async Task<int> CountAsync(Expression<Func<T, bool>> predicate) =>
        await _dbSet.Where(e => !e.IsDeleted).CountAsync(predicate);
}
