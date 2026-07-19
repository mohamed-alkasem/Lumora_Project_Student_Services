using System.Globalization;
using System.Security.Claims;
using Lumora.Application.DTOs.Consultations;
using Lumora.Application.Interfaces.Services;
using Lumora.Domain.Constants;
using Lumora.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Lumora.Web.Controllers;

/// Public consultation request ("Apply now" / "Free Consultation").
/// Requires a Student account by product decision — anonymous visitors are
/// sent to login/register first and bounced back here.
[Authorize(Roles = RoleConstants.Student)]
public class ConsultationsController : Controller
{
    private readonly IConsultationService _consultations;
    private readonly IUniversityService _universities;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public ConsultationsController(
        IConsultationService consultations,
        IUniversityService universities,
        UserManager<ApplicationUser> userManager,
        IStringLocalizer<SharedResources> localizer)
    {
        _consultations = consultations;
        _universities = universities;
        _userManager = userManager;
        _localizer = localizer;
    }

    [HttpGet]
    public async Task<IActionResult> Request(int? universityId)
    {
        var user = await _userManager.GetUserAsync(User);
        var dto = new ConsultationCreateDto
        {
            FullName = user?.FullName ?? string.Empty,
            Phone = user?.PhoneNumber ?? string.Empty,
            UniversityId = universityId
        };

        await LoadUniversityNameAsync(universityId);
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Request(ConsultationCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadUniversityNameAsync(dto.UniversityId);
            return View(dto);
        }

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _consultations.CreateAsync(userId, dto);

        TempData["Success"] = _localizer["RequestSent"].Value;
        return RedirectToAction("Index", "Universities");
    }

    private async Task LoadUniversityNameAsync(int? universityId)
    {
        if (universityId is int id)
        {
            var university = await _universities.GetByIdInLanguageAsync(
                id, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
            ViewBag.UniversityName = university?.Name;
        }
    }
}
