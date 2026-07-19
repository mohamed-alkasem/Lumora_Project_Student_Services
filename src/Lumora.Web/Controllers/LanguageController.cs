using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Lumora.Web.Controllers;

public class LanguageController : Controller
{
    [HttpGet]
    public IActionResult SetLanguage(string culture, string returnUrl)
    {
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true
            }
        );

        return LocalRedirect(returnUrl ?? "/");
    }
}
