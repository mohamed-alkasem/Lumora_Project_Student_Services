using System.Globalization;
using System.Security.Claims;
using Lumora.Application.DTOs.Universities;
using Lumora.Application.Interfaces.Services;
using Lumora.Domain.Constants;
using Lumora.Web.Models.Universities;
using Microsoft.AspNetCore.Mvc;

namespace Lumora.Web.Controllers;

public class UniversitiesController : Controller
{
    private readonly IUniversityService _universityService;
    private readonly ICityService _cityService;
    private readonly IFavoriteService _favoriteService;

    public UniversitiesController(
        IUniversityService universityService,
        ICityService cityService,
        IFavoriteService favoriteService)
    {
        _universityService = universityService;
        _cityService = cityService;
        _favoriteService = favoriteService;
    }

    private static string CurrentLanguage =>
        CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

    [HttpGet]
    public async Task<IActionResult> Index(UniversityFilterDto filter)
    {
        var lang = CurrentLanguage;
        var results = await _universityService.SearchAsync(filter, lang);
        var cities = await _cityService.GetAllInLanguageAsync(lang);
        var typeCounts = await _universityService.GetTypeCountsAsync();
        var favoriteIds = await GetFavoriteIdsAsync();

        return View(new UniversityIndexViewModel
        {
            Filter = filter,
            Cities = cities.ToList(),
            TypeCounts = typeCounts,
            List = new UniversityListViewModel { Results = results, FavoriteIds = favoriteIds },
            IsStudent = User.IsInRole(RoleConstants.Student)
        });
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var university = await _universityService.GetByIdInLanguageAsync(id, CurrentLanguage);
        if (university is null)
        {
            return NotFound();
        }

        var favoriteIds = await GetFavoriteIdsAsync();
        ViewBag.IsFavorite = favoriteIds.Contains(id);
        ViewBag.IsStudent = User.IsInRole(RoleConstants.Student);
        return View(university);
    }

    /// AJAX endpoint: returns only the grid fragment for filter updates.
    [HttpGet]
    public async Task<IActionResult> List(UniversityFilterDto filter)
    {
        var results = await _universityService.SearchAsync(filter, CurrentLanguage);
        var favoriteIds = await GetFavoriteIdsAsync();

        return PartialView("_UniversityList", new UniversityListViewModel
        {
            Results = results,
            FavoriteIds = favoriteIds
        });
    }

    private async Task<IReadOnlySet<int>> GetFavoriteIdsAsync()
    {
        if (!User.IsInRole(RoleConstants.Student))
        {
            return new HashSet<int>();
        }

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return await _favoriteService.GetMyFavoriteIdsAsync(userId);
    }
}
