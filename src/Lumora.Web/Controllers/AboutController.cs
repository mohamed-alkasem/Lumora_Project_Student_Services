using System.Globalization;
using Lumora.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lumora.Web.Controllers;

public class AboutController : Controller
{
    private readonly ITestimonialService _testimonials;

    public AboutController(ITestimonialService testimonials)
    {
        _testimonials = testimonials;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var testimonials = await _testimonials.GetPublishedAsync(
            CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
        return View(testimonials);
    }
}
