using Microsoft.AspNetCore.Mvc;

namespace Habitia.Areas.Admin.Controllers
{
    public class ViviendaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
