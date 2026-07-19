using System.Globalization;
using Lumora.Application.DTOs.Faqs;
using Lumora.Application.Interfaces.Services;
using Lumora.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Lumora.Web.Areas.Admin.Controllers;

public class FaqsController : AdminBaseController
{
    private readonly IFaqService _faqs;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public FaqsController(IFaqService faqs, IStringLocalizer<SharedResources> localizer)
    {
        _faqs = faqs;
        _localizer = localizer;
    }

    public async Task<IActionResult> Index() =>
        View(await _faqs.GetOrderedAsync(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName));

    [HttpGet]
    public IActionResult Create() =>
        View(new FaqCreateDto { Translations = TranslationForm.Normalize<FaqTranslationInputDto>(null) });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FaqCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        await _faqs.CreateAsync(dto);
        TempData["Success"] = _localizer["SaveSuccess"].Value;
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var dto = await _faqs.GetForEditAsync(id);
        if (dto is null)
        {
            return NotFound();
        }

        dto.Translations = TranslationForm.Normalize(dto.Translations);
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, FaqUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        if (await _faqs.UpdateAsync(id, dto) is null)
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
        if (await _faqs.DeleteAsync(id))
        {
            TempData["Success"] = _localizer["DeleteSuccess"].Value;
        }
        return RedirectToAction(nameof(Index));
    }
}
