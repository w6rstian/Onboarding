using Microsoft.AspNetCore.Mvc;

namespace Onboarding.Controllers
{
    public class CourseController : Controller
    {
        public IActionResult CreateCourse()
        {
            return View();
        }
    }
}
