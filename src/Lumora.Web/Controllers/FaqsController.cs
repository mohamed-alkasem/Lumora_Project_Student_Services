using System.Globalization;
using Lumora.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lumora.Web.Controllers;

public class FaqsController : Controller
{
    private readonly IFaqService _faqs;

    public FaqsController(IFaqService faqs)
    {
        _faqs = faqs;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var faqs = await _faqs.GetOrderedAsync(
            CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
        return View(faqs);
    }
}
