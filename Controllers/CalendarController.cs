using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Onboarding.Data;
using Onboarding.Models;
using Onboarding.ViewModels;

namespace Onboarding.Controllers
{
    public class CalendarController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public CalendarController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CreateCheckIn()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var newUsers = await _context.Users
                .Where(u => u.BuddyId == currentUser.Id)
                .ToListAsync();

            var model = new CheckInMeetingViewModel
            {
                NewUsers = newUsers.Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = $"{u.Name} {u.Surname} ({u.Email})"
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCheckIn(CheckInMeetingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                model.NewUsers = await _context.Users
                    .Where(u => u.BuddyId == currentUser.Id)
                    .Select(u => new SelectListItem
                    {
                        Value = u.Id.ToString(),
                        Text = $"{u.Name} {u.Surname} ({u.Email})"
                    }).ToListAsync();

                return View(model);
            }

            var buddy = await _userManager.GetUserAsync(User);

            var meeting = new CheckInMeeting
            {
                BuddyId = buddy.Id,
                NewUserId = model.NewUserId,
                Start = model.Start,
                End = model.End,
                Title = model.Title ?? "Check-In"
            };

            _context.Add(meeting);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index"); // kalendarz
        }

        [HttpGet]
        public async Task<JsonResult> GetEvents()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var meetings = await _context.CheckInMeetings
                .Where(m => m.BuddyId == currentUser.Id || m.NewUserId == currentUser.Id)
                .ToListAsync();

            var events = meetings.Select(m => new
            {
                title = m.Title ?? "Check-In",
                start = m.Start.ToString("s"),
                end = m.End.ToString("s")
            }).ToList();

            var exampleEvents = new[]
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

            events.AddRange(exampleEvents);

            return Json(events);
        }
    }
}
