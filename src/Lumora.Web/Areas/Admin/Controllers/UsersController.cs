using Lumora.Domain.Constants;
using Lumora.Infrastructure.Identity;
using Lumora.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Lumora.Web.Areas.Admin.Controllers;

/// Sub-admin management. Only the SuperAdmin can create admin accounts —
/// there is intentionally no public registration path for admins.
[Authorize(Policy = "SuperAdminOnly")]
public class UsersController : AdminBaseController
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public UsersController(
        UserManager<ApplicationUser> userManager,
        IStringLocalizer<SharedResources> localizer)
    {
        _userManager = userManager;
        _localizer = localizer;
    }

    public async Task<IActionResult> Index()
    {
        var rows = new List<AdminUserRowViewModel>();
        foreach (var role in RoleConstants.AdminRoles)
        {
            var users = await _userManager.GetUsersInRoleAsync(role);
            rows.AddRange(users.Select(u => new AdminUserRowViewModel
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email ?? string.Empty,
                Role = role,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt
            }));
        }

        return View(rows.OrderBy(r => r.CreatedAt).ToList());
    }

    [HttpGet]
    public IActionResult Create() => View(new CreateAdminViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateAdminViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FullName = model.FullName,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        await _userManager.AddToRoleAsync(user, RoleConstants.Admin);
        TempData["Success"] = _localizer["SaveSuccess"].Value;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null)
        {
            return NotFound();
        }

        // SuperAdmin accounts (including the seeded one and yourself) can never be deactivated here.
        if (await _userManager.IsInRoleAsync(user, RoleConstants.SuperAdmin))
        {
            TempData["Error"] = _localizer["AccessDenied"].Value;
            return RedirectToAction(nameof(Index));
        }

        user.IsActive = !user.IsActive;
        await _userManager.UpdateAsync(user);
        TempData["Success"] = _localizer["SaveSuccess"].Value;
        return RedirectToAction(nameof(Index));
    }
}
