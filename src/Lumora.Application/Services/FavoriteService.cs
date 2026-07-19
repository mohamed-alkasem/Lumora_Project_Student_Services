using Lumora.Application.DTOs.Universities;
using Lumora.Application.Interfaces.Services;
using Lumora.Application.Mapping;
using Lumora.Domain.Entities;
using Lumora.Domain.Interfaces.Repositories;

namespace Lumora.Application.Services;

public class FavoriteService : IFavoriteService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFavoriteRepository _favorites;
    private readonly IUniversityRepository _universities;

    public FavoriteService(
        IUnitOfWork unitOfWork,
        IFavoriteRepository favorites,
        IUniversityRepository universities)
    {
        _unitOfWork = unitOfWork;
        _favorites = favorites;
        _universities = universities;
    }

    public async Task<bool> ToggleAsync(int studentUserId, int universityId)
    {
        // Soft-deleted rows must be resurrected instead of re-inserted because of the
        // unique (StudentUserId, UniversityId) index.
        var existing = await _favorites.GetIncludingDeletedAsync(studentUserId, universityId);

        bool isNowFavorite;
        if (existing is null)
        {
            await _favorites.AddAsync(new Favorite
            {
                StudentUserId = studentUserId,
                UniversityId = universityId
            });
            isNowFavorite = true;
        }
        else
        {
            existing.IsDeleted = !existing.IsDeleted;
            existing.UpdatedAt = DateTime.UtcNow;
            isNowFavorite = !existing.IsDeleted;
        }

        await _unitOfWork.CommitAsync();
        return isNowFavorite;
    }

    public async Task<IReadOnlyList<UniversityCardDto>> GetMyFavoritesAsync(int studentUserId, string languageCode)
    {
        var favorites = await _favorites.FindAsync(f => f.StudentUserId == studentUserId);
        var orderedIds = favorites
            .OrderByDescending(f => f.CreatedAt)
            .Select(f => f.UniversityId)
            .ToList();

        if (orderedIds.Count == 0)
        {
            return [];
        }

        var universities = await _universities.GetByIdsWithDetailsAsync(orderedIds);
        var byId = universities.ToDictionary(u => u.Id);

        return orderedIds
            .Where(byId.ContainsKey)
            .Select(id => byId[id].ToCardDto(languageCode))
            .ToList();
    }

    public async Task<IReadOnlySet<int>> GetMyFavoriteIdsAsync(int studentUserId)
    {
        var favorites = await _favorites.FindAsync(f => f.StudentUserId == studentUserId);
        return favorites.Select(f => f.UniversityId).ToHashSet();
    }
}
