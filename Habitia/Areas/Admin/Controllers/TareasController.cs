using Microsoft.AspNetCore.Mvc;

namespace Habitia.Areas.Admin.Controllers
{
    public class TareasController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
