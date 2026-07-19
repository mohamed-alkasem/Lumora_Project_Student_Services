using System.Globalization;
using Lumora.Application.DTOs.Cities;
using Lumora.Application.Interfaces.Services;
using Lumora.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Lumora.Web.Areas.Admin.Controllers;

public class CitiesController : AdminBaseController
{
    private readonly ICityService _cities;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public CitiesController(ICityService cities, IStringLocalizer<SharedResources> localizer)
    {
        _cities = cities;
        _localizer = localizer;
    }

    public async Task<IActionResult> Index() =>
        View(await _cities.GetAllInLanguageAsync(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName));

    [HttpGet]
    public IActionResult Create() =>
        View(new CityCreateDto { Translations = TranslationForm.Normalize<CityTranslationInputDto>(null) });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CityCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        await _cities.CreateAsync(dto);
        TempData["Success"] = _localizer["SaveSuccess"].Value;
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var dto = await _cities.GetForEditAsync(id);
        if (dto is null)
        {
            return NotFound();
        }

        dto.Translations = TranslationForm.Normalize(dto.Translations);
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CityUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        if (await _cities.UpdateAsync(id, dto) is null)
        {
            return NotFound();
        }

        TempData["Success"] = _localizer["SaveSuccess"].Value;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (await _cities.DeleteAsync(id))
        {
            TempData["Success"] = _localizer["DeleteSuccess"].Value;
        }
        return RedirectToAction(nameof(Index));
    }
}
