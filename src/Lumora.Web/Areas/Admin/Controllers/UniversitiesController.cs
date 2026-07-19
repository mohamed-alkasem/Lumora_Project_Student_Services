using System.Globalization;
using Lumora.Application.DTOs.Universities;
using Lumora.Application.Interfaces.Services;
using Lumora.Domain.Interfaces.Services;
using Lumora.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;

namespace Lumora.Web.Areas.Admin.Controllers;

public class UniversitiesController : AdminBaseController
{
    private readonly IUniversityService _universities;
    private readonly ICityService _cities;
    private readonly IAcademicProgramService _programs;
    private readonly IFileStorageService _files;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public UniversitiesController(
        IUniversityService universities,
        ICityService cities,
        IAcademicProgramService programs,
        IFileStorageService files,
        IStringLocalizer<SharedResources> localizer)
    {
        _universities = universities;
        _cities = cities;
        _programs = programs;
        _files = files;
        _localizer = localizer;
    }

    private static string Lang => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

    public async Task<IActionResult> Index(int page = 1)
    {
        var results = await _universities.SearchAsync(
            new UniversityFilterDto { Page = page, PageSize = 20 }, Lang);
        return View(results);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadLookupsAsync();
        return View(new UniversityCreateDto { Translations = TranslationForm.Normalize<UniversityTranslationInputDto>(null) });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UniversityCreateDto dto, IFormFile? logoFile)
    {
        if (!await TryApplyLogoAsync(dto, logoFile) || !ModelState.IsValid)
        {
            await LoadLookupsAsync();
            return View(dto);
        }

        await _universities.CreateAsync(dto);
        TempData["Success"] = _localizer["SaveSuccess"].Value;
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var dto = await _universities.GetForEditAsync(id);
        if (dto is null)
        {
            return NotFound();
        }

        dto.Translations = TranslationForm.Normalize(dto.Translations);
        await LoadLookupsAsync();
        ViewBag.GalleryImages = await _universities.GetImagesAsync(id);
        ViewBag.EntityId = id;
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UniversityUpdateDto dto, IFormFile? logoFile)
    {
        if (!await TryApplyLogoAsync(dto, logoFile) || !ModelState.IsValid)
        {
            await LoadLookupsAsync();
            return View(dto);
        }

        if (await _universities.UpdateAsync(id, dto) is null)
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
        var dto = await _universities.GetForEditAsync(id);
        if (dto is not null && await _universities.DeleteAsync(id))
        {
            await _files.DeleteAsync(dto.LogoUrl);
            TempData["Success"] = _localizer["DeleteSuccess"].Value;
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadImages(int id, List<IFormFile> galleryFiles)
    {
        foreach (var file in galleryFiles.Where(f => f.Length > 0))
        {
            if (AdminUpload.Validate(file) is string errorKey)
            {
                TempData["Error"] = _localizer[errorKey].Value;
                continue;
            }

            await using var stream = file.OpenReadStream();
            var url = await _files.SaveAsync(stream, file.FileName, "universities");
            await _universities.AddImageAsync(id, url);
        }

        TempData["Success"] ??= _localizer["SaveSuccess"].Value;
        return RedirectToAction(nameof(Edit), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteImage(int id, int imageId)
    {
        var url = await _universities.RemoveImageAsync(imageId);
        await _files.DeleteAsync(url);
        TempData["Success"] = _localizer["DeleteSuccess"].Value;
        return RedirectToAction(nameof(Edit), new { id });
    }

    private async Task LoadLookupsAsync()
    {
        var cities = await _cities.GetAllInLanguageAsync(Lang);
        ViewBag.Cities = cities
            .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
            .ToList();

        var programs = await _programs.GetAllInLanguageAsync(Lang);
        ViewBag.Programs = programs.ToList();
    }

    private async Task<bool> TryApplyLogoAsync(UniversityCreateDto dto, IFormFile? logoFile)
    {
        if (logoFile is null)
        {
            return true;
        }

        if (AdminUpload.Validate(logoFile) is string errorKey)
        {
            ModelState.AddModelError(string.Empty, _localizer[errorKey].Value);
            return false;
        }

        var oldUrl = dto.LogoUrl;
        await using var stream = logoFile.OpenReadStream();
        dto.LogoUrl = await _files.SaveAsync(stream, logoFile.FileName, "universities");
        await _files.DeleteAsync(oldUrl);
        return true;
    }
}
