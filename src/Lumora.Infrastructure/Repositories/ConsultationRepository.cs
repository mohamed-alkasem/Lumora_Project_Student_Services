using Lumora.Domain.Common;
using Lumora.Domain.Entities;
using Lumora.Domain.Interfaces.Repositories;
using Lumora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lumora.Infrastructure.Repositories;

public class ConsultationRepository
    : GenericRepository<ConsultationRequest>, IConsultationRepository
{
    public ConsultationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<PagedResult<ConsultationRequest>> GetPagedAsync(ConsultationQueryParameters parameters)
    {
        var query = _dbSet.Where(c => !c.IsDeleted);

        if (parameters.Status.HasValue)
        {
            query = query.Where(c => c.Status == parameters.Status.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((parameters.Page - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .Include(c => c.University!).ThenInclude(u => u.Translations)
            .ToListAsync();

        return new PagedResult<ConsultationRequest>
        {
            Items = items,
            TotalCount = totalCount,
            Page = parameters.Page,
            PageSize = parameters.PageSize
        };
    }
}
