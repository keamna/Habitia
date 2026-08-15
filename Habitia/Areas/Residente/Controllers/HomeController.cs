using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Habitia.Areas.Residente.Controllers
{
    [Area("Residente")]
    [Authorize(Roles = "Residente")]
    public class HomeController : Controller
    {
        // La antigua vista Home/Index era solo un título de bienvenida.
        // Ahora la portada del rol es el Panel (Dashboard), así que se
        // redirige para no dejar dos páginas de inicio distintas.
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Dashboard", new { area = "Residente" });
        }
    }
}