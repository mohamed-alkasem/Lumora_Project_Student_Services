using System.Globalization;
using Lumora.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lumora.Web.Controllers;

public class ScholarshipsController : Controller
{
    private readonly IScholarshipService _scholarships;

    public ScholarshipsController(IScholarshipService scholarships)
    {
        _scholarships = scholarships;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        return View(await _scholarships.GetActiveAsync(lang));
    }
}
