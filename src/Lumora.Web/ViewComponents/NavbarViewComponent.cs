using Microsoft.AspNetCore.Mvc;

namespace Lumora.Web.ViewComponents;

public class NavbarViewComponent : ViewComponent
{
    public IViewComponentResult Invoke() => View();
}
