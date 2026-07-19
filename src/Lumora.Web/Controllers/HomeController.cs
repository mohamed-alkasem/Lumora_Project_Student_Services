using System.Diagnostics;
using System.Globalization;
using System.Security.Claims;
using Lumora.Application.DTOs.Universities;
using Lumora.Application.Interfaces.Services;
using Lumora.Domain.Constants;
using Lumora.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lumora.Web.Controllers;

public class HomeController : Controller
{
    private readonly IUniversityService _universities;
    private readonly ITestimonialService _testimonials;
    private readonly IFavoriteService _favorites;

    public HomeController(
        IUniversityService universities,
        ITestimonialService testimonials,
        IFavoriteService favorites)
    {
        _universities = universities;
        _testimonials = testimonials;
        _favorites = favorites;
    }

    public async Task<IActionResult> Index()
    {
        var lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

        var topUniversities = await _universities.SearchAsync(
            new UniversityFilterDto { PageSize = 6 }, lang); // TopRated is the default sort

        IReadOnlySet<int> favoriteIds = new HashSet<int>();
        if (User.IsInRole(RoleConstants.Student))
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            favoriteIds = await _favorites.GetMyFavoriteIdsAsync(userId);
        }

        ViewBag.FavoriteIds = favoriteIds;
        ViewBag.Testimonials = (await _testimonials.GetPublishedAsync(lang)).Take(12).ToList();
        return View(topUniversities.Items);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
