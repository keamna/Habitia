using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Habitia.Areas.Admin.Controllers
{


    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class DashboardController : Controller
    {


        public IActionResult Index()
        {

            ViewData["Title"] = "Panel Administrador";


            return View();

        }



    }


}