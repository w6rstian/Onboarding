using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Onboarding.Data;
using Onboarding.Data.Enums;
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
        public async Task<IActionResult> CreateMeeting(MeetingType type)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var users = await _context.Users
                .Where(u => u.Id != currentUser.Id)
                .ToListAsync();

            var model = new MeetingViewModel
            {
                Type = type,
                AllUsers = users.Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = $"{u.Name} {u.Surname} ({u.Email})"
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMeeting(MeetingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                model.AllUsers = await _context.Users
                    .Where(u => u.Id != currentUser.Id)
                    .Select(u => new SelectListItem
                    {
                        Value = u.Id.ToString(),
                        Text = $"{u.Name} {u.Surname} ({u.Email})"
                    }).ToListAsync();

                return View(model);
            }

            var organizer = await _userManager.GetUserAsync(User);

            var meeting = new Meeting
            {
                OrganizerId = organizer.Id,
                Start = model.Start,
                End = model.End,
                Title = model.Title ?? "Spotkanie",
                Type = model.Type
            };

            foreach (var userId in model.SelectedUsersIds)
            {
                var participant = new MeetingParticipant
                {
                    UserId = int.Parse(userId),
                };
                meeting.Participants.Add(participant);
            }

            _context.Add(meeting);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index"); // kalendarz
        }

        [HttpGet]
        public async Task<JsonResult> GetEvents(string? type)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var meetingsQuery = _context.Meetings
                .Include(m => m.Participants)
                .Where(m =>
                    m.OrganizerId == currentUser.Id ||
                    m.Participants.Any(mp => mp.UserId == currentUser.Id));

            if (!string.IsNullOrEmpty(type) && Enum.TryParse<MeetingType>(type, out var parsedType))
            {
                meetingsQuery = meetingsQuery.Where(m => m.Type == parsedType);
            }

            var meetings = await meetingsQuery.ToListAsync();

            var events = meetings.Select(m => new
            {
                title = m.Title ?? "Spotkanie",
                start = m.Start.ToString("s"),
                end = m.End.ToString("s")
            }).ToList();

            return Json(events);
        }
    }
}
