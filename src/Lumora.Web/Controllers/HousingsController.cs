using System.Globalization;
using Lumora.Application.DTOs.Housings;
using Lumora.Application.Interfaces.Services;
using Lumora.Web.Models.Housings;
using Microsoft.AspNetCore.Mvc;

namespace Lumora.Web.Controllers;

public class HousingsController : Controller
{
    private readonly IHousingService _housingService;
    private readonly ICityService _cityService;

    public HousingsController(IHousingService housingService, ICityService cityService)
    {
        _housingService = housingService;
        _cityService = cityService;
    }

    private static string CurrentLanguage =>
        CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

    [HttpGet]
    public async Task<IActionResult> Index(HousingFilterDto filter)
    {
        var lang = CurrentLanguage;
        var results = await _housingService.SearchAsync(filter, lang);
        var cities = await _cityService.GetAllInLanguageAsync(lang);

        return View(new HousingIndexViewModel
        {
            Filter = filter,
            Cities = cities.ToList(),
            Results = results
        });
    }

    /// AJAX endpoint: returns only the grid fragment for filter updates.
    [HttpGet]
    public async Task<IActionResult> List(HousingFilterDto filter)
    {
        var results = await _housingService.SearchAsync(filter, CurrentLanguage);
        return PartialView("_HousingList", results);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var housing = await _housingService.GetByIdInLanguageAsync(id, CurrentLanguage);
        if (housing is null)
        {
            return NotFound();
        }

        return View(housing);
    }
}
