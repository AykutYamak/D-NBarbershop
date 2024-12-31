using Microsoft.AspNetCore.Mvc;

namespace DNBarbershop.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
