using Lumora.Domain.Common;
using Lumora.Domain.Entities;

namespace Lumora.Domain.Interfaces.Repositories;

public interface IConsultationRepository : IGenericRepository<ConsultationRequest>
{
    /// Admin inbox: filtered by status, newest first, paged. Includes University.Translations for display.
    Task<PagedResult<ConsultationRequest>> GetPagedAsync(ConsultationQueryParameters parameters);
}
