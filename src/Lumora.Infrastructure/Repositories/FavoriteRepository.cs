using Lumora.Domain.Entities;
using Lumora.Domain.Interfaces.Repositories;
using Lumora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lumora.Infrastructure.Repositories;

public class FavoriteRepository : GenericRepository<Favorite>, IFavoriteRepository
{
    public FavoriteRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Favorite?> GetIncludingDeletedAsync(int studentUserId, int universityId) =>
        await _dbSet.FirstOrDefaultAsync(f =>
            f.StudentUserId == studentUserId && f.UniversityId == universityId);
}
