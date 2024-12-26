using Microsoft.AspNetCore.Mvc;

namespace DNBarbershop.Controllers
{
    public class BarberController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
