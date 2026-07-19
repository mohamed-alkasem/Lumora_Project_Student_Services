using Microsoft.AspNetCore.Mvc;

namespace Lumora.Web.ViewComponents;

public class FooterViewComponent : ViewComponent
{
    public IViewComponentResult Invoke() => View();
}
