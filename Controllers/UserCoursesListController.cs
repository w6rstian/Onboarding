using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Onboarding.Data;
using Onboarding.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Onboarding.Controllers
{
    [Authorize] // Upewnia się, że tylko zalogowani użytkownicy mają dostęp
    public class UserCoursesListController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserCoursesListController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Pobieranie ID zalogowanego użytkownika
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Pobieranie kursów przypisanych do użytkownika
            var userCourses = await _context.UserCourses
                .Where(uc => uc.UserId == userId)
                .Include(uc => uc.Course)
                .Select(uc => uc.Course)
                .ToListAsync();

            return View(userCourses);
        }
    }
}
