using Microsoft.AspNetCore.Mvc;

namespace Onboarding.Controllers
{
    public class CalendarController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetEvents()
        {
            // Na razie zwracamy przykładowe dane
            var events = new[]
            {
            new {
                title = "Spotkanie zespołu",
                start = "2025-05-10T10:00:00",
                end = "2025-05-10T11:00:00"
            },
            new {
                title = "Lunch z klientem",
                start = "2025-05-12T13:00:00",
                end = "2025-05-12T14:00:00"
            }
        };

            return Json(events);
        }
    }
}
