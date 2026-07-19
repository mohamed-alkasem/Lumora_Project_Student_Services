using Lumora.Domain.Common;
using Lumora.Domain.Entities;
using Lumora.Domain.Interfaces.Repositories;
using Lumora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lumora.Infrastructure.Repositories;

public class HousingRepository
    : TranslatableRepository<Housing, HousingTranslation>, IHousingRepository
{
    public HousingRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<PagedResult<Housing>> SearchAsync(HousingQueryParameters parameters)
    {
        var query = _dbSet.Where(h => !h.IsDeleted);

        if (parameters.CityId.HasValue)
        {
            query = query.Where(h => h.CityId == parameters.CityId.Value);
        }

        if (parameters.MaxPricePerMonthUsd.HasValue)
        {
            query = query.Where(h => h.PricePerMonthUsd <= parameters.MaxPricePerMonthUsd.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(h => h.CreatedAt)
            .Skip((parameters.Page - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .Include(h => h.Translations)
            .Include(h => h.City).ThenInclude(c => c.Translations)
            .AsSplitQuery()
            .ToListAsync();

        return new PagedResult<Housing>
        {
            Items = items,
            TotalCount = totalCount,
            Page = parameters.Page,
            PageSize = parameters.PageSize
        };
    }

    public async Task<Housing?> GetWithDetailsAsync(int id) =>
        await _dbSet
            .Include(h => h.Translations)
            .Include(h => h.City).ThenInclude(c => c.Translations)
            .Include(h => h.Images.Where(i => !i.IsDeleted).OrderBy(i => i.DisplayOrder))
            .AsSplitQuery()
            .FirstOrDefaultAsync(h => h.Id == id && !h.IsDeleted);
}
