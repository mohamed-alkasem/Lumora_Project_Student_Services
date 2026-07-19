using System.Globalization;
using Lumora.Application.DTOs.Consultations;
using Lumora.Application.Interfaces.Services;
using Lumora.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Lumora.Web.Areas.Admin.Controllers;

public class ConsultationsController : AdminBaseController
{
    private readonly IConsultationService _consultations;

    public ConsultationsController(IConsultationService consultations)
    {
        _consultations = consultations;
    }

    public async Task<IActionResult> Index(ConsultationStatus? status, int page = 1)
    {
        var results = await _consultations.GetPagedForAdminAsync(
            new ConsultationFilterDto { Status = status, Page = page },
            CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

        ViewBag.CurrentStatus = status;
        return View(results);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, ConsultationStatus status, ConsultationStatus? filter)
    {
        await _consultations.UpdateStatusAsync(id, status);
        return RedirectToAction(nameof(Index), new { status = filter });
    }
}
