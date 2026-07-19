using Lumora.Domain.Common;
using Lumora.Domain.Entities;

namespace Lumora.Domain.Interfaces.Repositories;

public interface IHousingRepository : ITranslatableRepository<Housing, HousingTranslation>
{
    /// Filtered + paged search. Results include Translations and City.Translations.
    Task<PagedResult<Housing>> SearchAsync(HousingQueryParameters parameters);

    Task<Housing?> GetWithDetailsAsync(int id);
}
