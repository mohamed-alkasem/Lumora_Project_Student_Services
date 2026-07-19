using System.Globalization;
using Lumora.Application.DTOs.Scholarships;
using Lumora.Application.Interfaces.Services;
using Lumora.Domain.Interfaces.Services;
using Lumora.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;

namespace Lumora.Web.Areas.Admin.Controllers;

public class ScholarshipsController : AdminBaseController
{
    private readonly IScholarshipService _scholarships;
    private readonly IUniversityService _universities;
    private readonly IFileStorageService _files;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public ScholarshipsController(
        IScholarshipService scholarships,
        IUniversityService universities,
        IFileStorageService files,
        IStringLocalizer<SharedResources> localizer)
    {
        _scholarships = scholarships;
        _universities = universities;
        _files = files;
        _localizer = localizer;
    }

    private static string Lang => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

    public async Task<IActionResult> Index() =>
        View(await _scholarships.GetAllInLanguageAsync(Lang));

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadUniversitiesAsync();
        return View(new ScholarshipCreateDto { Translations = TranslationForm.Normalize<ScholarshipTranslationInputDto>(null) });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ScholarshipCreateDto dto, IFormFile? imageFile)
    {
        if (!await TryApplyImageAsync(dto, imageFile) || !ModelState.IsValid)
        {
            await LoadUniversitiesAsync();
            return View(dto);
        }

        await _scholarships.CreateAsync(dto);
        TempData["Success"] = _localizer["SaveSuccess"].Value;
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var dto = await _scholarships.GetForEditAsync(id);
        if (dto is null)
        {
            return NotFound();
        }

        dto.Translations = TranslationForm.Normalize(dto.Translations);
        await LoadUniversitiesAsync();
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ScholarshipUpdateDto dto, IFormFile? imageFile)
    {
        if (!await TryApplyImageAsync(dto, imageFile) || !ModelState.IsValid)
        {
            await LoadUniversitiesAsync();
            return View(dto);
        }

        if (await _scholarships.UpdateAsync(id, dto) is null)
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
        var dto = await _scholarships.GetForEditAsync(id);
        if (dto is not null && await _scholarships.DeleteAsync(id))
        {
            await _files.DeleteAsync(dto.ImageUrl);
            TempData["Success"] = _localizer["DeleteSuccess"].Value;
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadUniversitiesAsync()
    {
        var universities = await _universities.GetAllInLanguageAsync(Lang);
        ViewBag.Universities = universities
            .Select(u => new SelectListItem(u.Name, u.Id.ToString()))
            .ToList();
    }

    private async Task<bool> TryApplyImageAsync(ScholarshipCreateDto dto, IFormFile? imageFile)
    {
        if (imageFile is null)
        {
            return true;
        }

        if (AdminUpload.Validate(imageFile) is string errorKey)
        {
            ModelState.AddModelError(string.Empty, _localizer[errorKey].Value);
            return false;
        }

        var oldUrl = dto.ImageUrl;
        await using var stream = imageFile.OpenReadStream();
        dto.ImageUrl = await _files.SaveAsync(stream, imageFile.FileName, "scholarships");
        await _files.DeleteAsync(oldUrl);
        return true;
    }
}
