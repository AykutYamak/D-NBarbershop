using Microsoft.AspNetCore.Mvc;

namespace DNBarbershop.Controllers
{
    public class AppointmentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
