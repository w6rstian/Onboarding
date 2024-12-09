using Microsoft.AspNetCore.Mvc;

namespace Onboarding.Controllers
{
    public class HRController : Controller
    {
        public IActionResult CreateEmployee()
        {
            return View();
        }
    }
}
