using Lumora.Domain.Common;
using Lumora.Domain.Entities;

namespace Lumora.Domain.Interfaces.Repositories;

public interface IUniversityRepository : ITranslatableRepository<University, UniversityTranslation>
{
    /// Filtered + sorted + paged search. Results include Translations, City.Translations and Programs.Translations.
    Task<PagedResult<University>> SearchAsync(UniversityQueryParameters parameters);

    Task<University?> GetWithDetailsAsync(int id);

    /// Used by Favorites to load a student's saved universities as full cards.
    Task<IReadOnlyList<University>> GetByIdsWithDetailsAsync(IReadOnlyCollection<int> ids);
}
