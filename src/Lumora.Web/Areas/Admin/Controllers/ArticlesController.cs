using System.Globalization;
using Lumora.Application.DTOs.Articles;
using Lumora.Application.Interfaces.Services;
using Lumora.Domain.Interfaces.Services;
using Lumora.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Lumora.Web.Areas.Admin.Controllers;

public class ArticlesController : AdminBaseController
{
    private readonly IArticleService _articles;
    private readonly IFileStorageService _files;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public ArticlesController(
        IArticleService articles,
        IFileStorageService files,
        IStringLocalizer<SharedResources> localizer)
    {
        _articles = articles;
        _files = files;
        _localizer = localizer;
    }

    private static string Lang => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

    public async Task<IActionResult> Index() =>
        View(await _articles.GetAllInLanguageAsync(Lang));

    [HttpGet]
    public IActionResult Create() =>
        View(new ArticleCreateDto { Translations = TranslationForm.Normalize<ArticleTranslationInputDto>(null) });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ArticleCreateDto dto, IFormFile? coverFile)
    {
        if (!await TryApplyCoverAsync(dto, coverFile) || !ModelState.IsValid)
        {
            return View(dto);
        }

        await _articles.CreateAsync(dto);
        TempData["Success"] = _localizer["SaveSuccess"].Value;
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var dto = await _articles.GetForEditAsync(id);
        if (dto is null)
        {
            return NotFound();
        }

        dto.Translations = TranslationForm.Normalize(dto.Translations);
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ArticleUpdateDto dto, IFormFile? coverFile)
    {
        if (!await TryApplyCoverAsync(dto, coverFile) || !ModelState.IsValid)
        {
            return View(dto);
        }

        if (await _articles.UpdateAsync(id, dto) is null)
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
        var dto = await _articles.GetForEditAsync(id);
        if (dto is not null && await _articles.DeleteAsync(id))
        {
            await _files.DeleteAsync(dto.CoverImageUrl);
            TempData["Success"] = _localizer["DeleteSuccess"].Value;
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task<bool> TryApplyCoverAsync(ArticleCreateDto dto, IFormFile? coverFile)
    {
        if (coverFile is null)
        {
            return true;
        }

        if (AdminUpload.Validate(coverFile) is string errorKey)
        {
            ModelState.AddModelError(string.Empty, _localizer[errorKey].Value);
            return false;
        }

        var oldUrl = dto.CoverImageUrl;
        await using var stream = coverFile.OpenReadStream();
        dto.CoverImageUrl = await _files.SaveAsync(stream, coverFile.FileName, "articles");
        await _files.DeleteAsync(oldUrl);
        return true;
    }
}
