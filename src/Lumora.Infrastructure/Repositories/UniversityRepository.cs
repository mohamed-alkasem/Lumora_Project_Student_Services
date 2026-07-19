using Lumora.Domain.Common;
using Lumora.Domain.Entities;
using Lumora.Domain.Enums;
using Lumora.Domain.Interfaces.Repositories;
using Lumora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lumora.Infrastructure.Repositories;

public class UniversityRepository
    : TranslatableRepository<University, UniversityTranslation>, IUniversityRepository
{
    public UniversityRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<PagedResult<University>> SearchAsync(UniversityQueryParameters parameters)
    {
        var query = _dbSet.Where(u => !u.IsDeleted);

        if (parameters.Type.HasValue)
        {
            query = query.Where(u => u.Type == parameters.Type.Value);
        }

        if (parameters.CityId.HasValue)
        {
            query = query.Where(u => u.CityId == parameters.CityId.Value);
        }

        if (parameters.MaxAnnualTuitionUsd.HasValue)
        {
            query = query.Where(u => u.AnnualTuitionUsd <= parameters.MaxAnnualTuitionUsd.Value);
        }

        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            // Matches any language so a user typing in Arabic finds the university
            // even while browsing the English UI, mirroring the "search by university
            // or program" behavior in the design.
            var term = parameters.SearchTerm.Trim();
            query = query.Where(u =>
                u.Translations.Any(t => t.Name.Contains(term)) ||
                u.Programs.Any(p => p.Translations.Any(t => t.Name.Contains(term))));
        }

        var totalCount = await query.CountAsync();

        query = parameters.SortBy switch
        {
            UniversitySortOption.TuitionAsc  => query.OrderBy(u => u.AnnualTuitionUsd).ThenByDescending(u => u.Rating),
            UniversitySortOption.TuitionDesc => query.OrderByDescending(u => u.AnnualTuitionUsd).ThenByDescending(u => u.Rating),
            UniversitySortOption.Newest      => query.OrderByDescending(u => u.CreatedAt),
            _                                => query.OrderByDescending(u => u.Rating).ThenBy(u => u.AnnualTuitionUsd)
        };

        var items = await query
            .Skip((parameters.Page - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .Include(u => u.Translations)
            .Include(u => u.City).ThenInclude(c => c.Translations)
            .Include(u => u.Programs).ThenInclude(p => p.Translations)
            .AsSplitQuery()
            .ToListAsync();

        return new PagedResult<University>
        {
            Items = items,
            TotalCount = totalCount,
            Page = parameters.Page,
            PageSize = parameters.PageSize
        };
    }

    public async Task<University?> GetWithDetailsAsync(int id) =>
        await _dbSet
            .Include(u => u.Translations)
            .Include(u => u.City).ThenInclude(c => c.Translations)
            .Include(u => u.Programs).ThenInclude(p => p.Translations)
            .Include(u => u.Images.Where(i => !i.IsDeleted).OrderBy(i => i.DisplayOrder))
            .AsSplitQuery()
            .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);

    public async Task<IReadOnlyList<University>> GetByIdsWithDetailsAsync(IReadOnlyCollection<int> ids) =>
        await _dbSet
            .Where(u => ids.Contains(u.Id) && !u.IsDeleted)
            .Include(u => u.Translations)
            .Include(u => u.City).ThenInclude(c => c.Translations)
            .Include(u => u.Programs).ThenInclude(p => p.Translations)
            .AsSplitQuery()
            .ToListAsync();
}
