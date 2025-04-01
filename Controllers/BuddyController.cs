using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Onboarding.Data;
using Onboarding.Models;

namespace Onboarding.Controllers
{
    public class BuddyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public BuddyController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> TaskStatus()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var tasks = _context.UserTasks
                .Where(t => t.user.Buddy == currentUser)
                .ToList();


            // DEBUG
            var nowi = await _userManager.GetUsersInRoleAsync("Nowy");
            var nowy1 = nowi.FirstOrDefault(t => t.Email == "nowy1@mail.com");


            var tempUserTask = new UserTask
            {
                user = nowy1,
                Status = StatusTask.InProgress,
                Task = new Models.Task
                {
                    Title = "Testowy task",
                    Description = "Ten task jest sztuczny"
                }
            };

            if (currentUser == nowy1.Buddy)
            {
                tasks.Add(tempUserTask);
            }

            // DEBUG END

            return View(tasks);
        }
    }
}
