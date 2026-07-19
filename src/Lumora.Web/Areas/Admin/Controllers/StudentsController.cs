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

    public async Task<IActionResult> Index()
    {
        var students = await _userManager.GetUsersInRoleAsync(RoleConstants.Student);
        return View(students.OrderByDescending(s => s.CreatedAt).ToList());
    }
}
