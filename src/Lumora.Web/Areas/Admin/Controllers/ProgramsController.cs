using System.Globalization;
using Lumora.Application.DTOs.Programs;
using Lumora.Application.Interfaces.Services;
using Lumora.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Lumora.Web.Areas.Admin.Controllers;

public class ProgramsController : AdminBaseController
{
    private readonly IAcademicProgramService _programs;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public ProgramsController(IAcademicProgramService programs, IStringLocalizer<SharedResources> localizer)
    {
        _programs = programs;
        _localizer = localizer;
    }

    public async Task<IActionResult> Index() =>
        View(await _programs.GetAllInLanguageAsync(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName));

    [HttpGet]
    public IActionResult Create() =>
        View(new AcademicProgramCreateDto { Translations = TranslationForm.Normalize<AcademicProgramTranslationInputDto>(null) });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AcademicProgramCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        await _programs.CreateAsync(dto);
        TempData["Success"] = _localizer["SaveSuccess"].Value;
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var dto = await _programs.GetForEditAsync(id);
        if (dto is null)
        {
            return NotFound();
        }

        dto.Translations = TranslationForm.Normalize(dto.Translations);
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AcademicProgramUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        if (await _programs.UpdateAsync(id, dto) is null)
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
        if (await _programs.DeleteAsync(id))
        {
            TempData["Success"] = _localizer["DeleteSuccess"].Value;
        }
        return RedirectToAction(nameof(Index));
    }
}
