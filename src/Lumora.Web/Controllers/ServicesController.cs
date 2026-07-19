using Microsoft.AspNetCore.Mvc;

namespace Lumora.Web.Controllers;

public class ServicesController : Controller
{
    [HttpGet]
    public IActionResult Index() => View();
}
