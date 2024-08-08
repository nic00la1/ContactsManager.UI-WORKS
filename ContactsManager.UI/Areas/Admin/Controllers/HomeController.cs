using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.UI.Areas.Admin.Controllers;

[Area("Admin")]
[Route("[area]/[controller]/[action]")]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
