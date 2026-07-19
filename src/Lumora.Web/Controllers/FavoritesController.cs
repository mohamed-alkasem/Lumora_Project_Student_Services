using System.Globalization;
using System.Security.Claims;
using Lumora.Application.Interfaces.Services;
using Lumora.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lumora.Web.Controllers;

[Authorize(Roles = RoleConstants.Student)]
public class FavoritesController : Controller
{
    private readonly IFavoriteService _favoriteService;

    public FavoritesController(IFavoriteService favoriteService)
    {
        _favoriteService = favoriteService;
    }

    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var favorites = await _favoriteService.GetMyFavoritesAsync(
            CurrentUserId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
        return View(favorites);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(int universityId)
    {
        var isFavorite = await _favoriteService.ToggleAsync(CurrentUserId, universityId);
        var ids = await _favoriteService.GetMyFavoriteIdsAsync(CurrentUserId);

        return Json(new { isFavorite, count = ids.Count });
    }
}
