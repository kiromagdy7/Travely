using Microsoft.AspNetCore.Mvc;

namespace Travely.Controllers
{
    public class ContactUs : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
