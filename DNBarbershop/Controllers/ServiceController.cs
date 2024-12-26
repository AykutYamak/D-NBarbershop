using Microsoft.AspNetCore.Mvc;

namespace DNBarbershop.Controllers
{
    public class ServiceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
