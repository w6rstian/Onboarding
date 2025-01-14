using Microsoft.AspNetCore.Mvc;

namespace Onboarding.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AdminPanel()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }
    }
}
