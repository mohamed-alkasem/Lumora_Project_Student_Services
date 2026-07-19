using Lumora.Domain.Entities;

namespace Lumora.Domain.Interfaces.Repositories;

public interface IFavoriteRepository : IGenericRepository<Favorite>
{
    /// Includes soft-deleted rows: the unique (StudentUserId, UniversityId) index covers them too,
    /// so toggling a favorite back on must resurrect the existing row instead of inserting a duplicate.
    Task<Favorite?> GetIncludingDeletedAsync(int studentUserId, int universityId);
}
