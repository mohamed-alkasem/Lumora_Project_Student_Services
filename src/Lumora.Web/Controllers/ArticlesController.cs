using System.Globalization;
using Lumora.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lumora.Web.Controllers;

public class ArticlesController : Controller
{
    private readonly IArticleService _articles;

    public ArticlesController(IArticleService articles)
    {
        _articles = articles;
    }

    private static string Lang => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

    [HttpGet]
    public async Task<IActionResult> Index() =>
        View(await _articles.GetPublishedAsync(Lang));

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var article = await _articles.GetByIdInLanguageAsync(id, Lang);
        if (article is null || !article.IsPublished)
        {
            return NotFound();
        }

        return View(article);
    }
}
