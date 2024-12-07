using Microsoft.AspNetCore.Mvc;

namespace Onboarding.Controllers
{
    public class UserController : Controller
    {
        public IActionResult CreateEmployee()
        {
            return View();
        }
        public IActionResult MainPage()
        {
            return View();
        }
    }

}
