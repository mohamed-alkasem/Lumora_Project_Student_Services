using Lumora.Domain.Constants;
using Lumora.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Lumora.Web.Areas.Admin.Controllers;

public class StudentsController : AdminBaseController
{
    private readonly UserManager<ApplicationUser> _userManager;

    public StudentsController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(int page = 1)
    {
        const int pageSize = 20;
        var students = await _userManager.GetUsersInRoleAsync(RoleConstants.Student);
        var ordered = students.OrderByDescending(s => s.CreatedAt).ToList();

        ViewBag.Page = page;
        ViewBag.TotalPages = (int)Math.Ceiling(ordered.Count / (double)pageSize);
        ViewBag.TotalCount = ordered.Count;
        return View(ordered.Skip((page - 1) * pageSize).Take(pageSize).ToList());
    }
}
