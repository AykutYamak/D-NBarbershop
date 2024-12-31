using Microsoft.AspNetCore.Mvc;

namespace DNBarbershop.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
