using Microsoft.AspNetCore.Mvc;

namespace DNBarbershop.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
