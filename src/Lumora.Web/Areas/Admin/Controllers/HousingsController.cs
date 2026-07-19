using System.Globalization;
using Lumora.Application.DTOs.Housings;
using Lumora.Application.Interfaces.Services;
using Lumora.Domain.Interfaces.Services;
using Lumora.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;

namespace Lumora.Web.Areas.Admin.Controllers;

public class HousingsController : AdminBaseController
{
    private readonly IHousingService _housings;
    private readonly ICityService _cities;
    private readonly IFileStorageService _files;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public HousingsController(
        IHousingService housings,
        ICityService cities,
        IFileStorageService files,
        IStringLocalizer<SharedResources> localizer)
    {
        _housings = housings;
        _cities = cities;
        _files = files;
        _localizer = localizer;
    }

    private static string Lang => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

    public async Task<IActionResult> Index() =>
        View(await _housings.GetAllInLanguageAsync(Lang));

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadCitiesAsync();
        return View(new HousingCreateDto { Translations = TranslationForm.Normalize<HousingTranslationInputDto>(null) });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(HousingCreateDto dto, IFormFile? imageFile)
    {
        if (!await TryApplyImageAsync(dto, imageFile) || !ModelState.IsValid)
        {
            await LoadCitiesAsync();
            return View(dto);
        }

        await _housings.CreateAsync(dto);
        TempData["Success"] = _localizer["SaveSuccess"].Value;
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var dto = await _housings.GetForEditAsync(id);
        if (dto is null)
        {
            return NotFound();
        }

        dto.Translations = TranslationForm.Normalize(dto.Translations);
        await LoadCitiesAsync();
        ViewBag.GalleryImages = await _housings.GetImagesAsync(id);
        ViewBag.EntityId = id;
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, HousingUpdateDto dto, IFormFile? imageFile)
    {
        if (!await TryApplyImageAsync(dto, imageFile) || !ModelState.IsValid)
        {
            await LoadCitiesAsync();
            return View(dto);
        }

        if (await _housings.UpdateAsync(id, dto) is null)
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
        var dto = await _housings.GetForEditAsync(id);
        if (dto is not null && await _housings.DeleteAsync(id))
        {
            await _files.DeleteAsync(dto.ImageUrl);
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
            var url = await _files.SaveAsync(stream, file.FileName, "housings");
            await _housings.AddImageAsync(id, url);
        }

        TempData["Success"] ??= _localizer["SaveSuccess"].Value;
        return RedirectToAction(nameof(Edit), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteImage(int id, int imageId)
    {
        var url = await _housings.RemoveImageAsync(imageId);
        await _files.DeleteAsync(url);
        TempData["Success"] = _localizer["DeleteSuccess"].Value;
        return RedirectToAction(nameof(Edit), new { id });
    }

    private async Task LoadCitiesAsync()
    {
        var cities = await _cities.GetAllInLanguageAsync(Lang);
        ViewBag.Cities = cities
            .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
            .ToList();
    }

    private async Task<bool> TryApplyImageAsync(HousingCreateDto dto, IFormFile? imageFile)
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
        dto.ImageUrl = await _files.SaveAsync(stream, imageFile.FileName, "housings");
        await _files.DeleteAsync(oldUrl);
        return true;
    }
}
