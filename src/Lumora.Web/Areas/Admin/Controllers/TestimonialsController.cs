using System.Globalization;
using Lumora.Application.DTOs.Testimonials;
using Lumora.Application.Interfaces.Services;
using Lumora.Domain.Interfaces.Services;
using Lumora.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Lumora.Web.Areas.Admin.Controllers;

public class TestimonialsController : AdminBaseController
{
    private readonly ITestimonialService _testimonials;
    private readonly IFileStorageService _files;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public TestimonialsController(
        ITestimonialService testimonials,
        IFileStorageService files,
        IStringLocalizer<SharedResources> localizer)
    {
        _testimonials = testimonials;
        _files = files;
        _localizer = localizer;
    }

    public async Task<IActionResult> Index() =>
        View(await _testimonials.GetAllInLanguageAsync(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName));

    [HttpGet]
    public IActionResult Create() =>
        View(new TestimonialCreateDto { Translations = TranslationForm.Normalize<TestimonialTranslationInputDto>(null) });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TestimonialCreateDto dto, IFormFile? photoFile)
    {
        if (!await TryApplyPhotoAsync(dto, photoFile) || !ModelState.IsValid)
        {
            return View(dto);
        }

        await _testimonials.CreateAsync(dto);
        TempData["Success"] = _localizer["SaveSuccess"].Value;
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var dto = await _testimonials.GetForEditAsync(id);
        if (dto is null)
        {
            return NotFound();
        }

        dto.Translations = TranslationForm.Normalize(dto.Translations);
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TestimonialUpdateDto dto, IFormFile? photoFile)
    {
        if (!await TryApplyPhotoAsync(dto, photoFile) || !ModelState.IsValid)
        {
            return View(dto);
        }

        if (await _testimonials.UpdateAsync(id, dto) is null)
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
        var dto = await _testimonials.GetForEditAsync(id);
        if (dto is not null && await _testimonials.DeleteAsync(id))
        {
            await _files.DeleteAsync(dto.PhotoUrl);
            TempData["Success"] = _localizer["DeleteSuccess"].Value;
        }
        return RedirectToAction(nameof(Index));
    }

    /// Saves the new photo (if any) and replaces dto.PhotoUrl. Keeps the old URL when no file was sent.
    private async Task<bool> TryApplyPhotoAsync(TestimonialCreateDto dto, IFormFile? photoFile)
    {
        if (photoFile is null)
        {
            return true;
        }

        if (AdminUpload.Validate(photoFile) is string errorKey)
        {
            ModelState.AddModelError(string.Empty, _localizer[errorKey].Value);
            return false;
        }

        var oldUrl = dto.PhotoUrl;
        await using var stream = photoFile.OpenReadStream();
        dto.PhotoUrl = await _files.SaveAsync(stream, photoFile.FileName, "testimonials");
        await _files.DeleteAsync(oldUrl);
        return true;
    }
}
