using System.Globalization;
using Lumora.Application.DTOs.Consultations;
using Lumora.Application.Interfaces.Services;
using Lumora.Domain.Constants;
using Lumora.Domain.Enums;
using Lumora.Infrastructure.Identity;
using Lumora.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Lumora.Web.Areas.Admin.Controllers;

public class HomeController : AdminBaseController
{
    private readonly IUniversityService _universities;
    private readonly ICityService _cities;
    private readonly IAcademicProgramService _programs;
    private readonly IHousingService _housings;
    private readonly IFaqService _faqs;
    private readonly ITestimonialService _testimonials;
    private readonly IConsultationService _consultations;
    private readonly UserManager<ApplicationUser> _userManager;

    public HomeController(
        IUniversityService universities,
        ICityService cities,
        IAcademicProgramService programs,
        IHousingService housings,
        IFaqService faqs,
        ITestimonialService testimonials,
        IConsultationService consultations,
        UserManager<ApplicationUser> userManager)
    {
        _universities = universities;
        _cities = cities;
        _programs = programs;
        _housings = housings;
        _faqs = faqs;
        _testimonials = testimonials;
        _consultations = consultations;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        var typeCounts = await _universities.GetTypeCountsAsync();
        var newConsultations = await _consultations.GetPagedForAdminAsync(
            new ConsultationFilterDto { Status = ConsultationStatus.New, PageSize = 1 }, lang);
        var students = await _userManager.GetUsersInRoleAsync(RoleConstants.Student);

        return View(new DashboardViewModel
        {
            Universities = typeCounts.All,
            Cities = (await _cities.GetAllAsync()).Count(),
            Programs = (await _programs.GetAllAsync()).Count(),
            Housings = (await _housings.GetAllAsync()).Count(),
            Faqs = (await _faqs.GetAllAsync()).Count(),
            Testimonials = (await _testimonials.GetAllAsync()).Count(),
            Students = students.Count,
            NewConsultations = newConsultations.TotalCount
        });
    }
}
