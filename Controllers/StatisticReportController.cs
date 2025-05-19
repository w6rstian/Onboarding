using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Onboarding.Data;
using Onboarding.Data.Enums;
using Onboarding.Models;
using Onboarding.ViewModels;
//using QuestPDF.Fluent;
//using QuestPDF.Helpers;
//using QuestPDF.Infrastructure;

namespace Onboarding.Controllers
{
    public class StatisticReportController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public StatisticReportController(ApplicationDbContext context, UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            // Pobierz wszystkie role
            var roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();

            // Pobierz wszystkich użytkowników wraz z rolami
            var users = await _context.Users.ToListAsync();

            // Pobierz role użytkowników (asynchronicznie)
            var userRolesDict = new Dictionary<int, IList<string>>();
            foreach (var user in users)
            {
                var rolesForUser = await _userManager.GetRolesAsync(user);
                userRolesDict[user.Id] = rolesForUser;
            }

            // Liczba użytkowników na role
            var userCountsByRole = roles.ToDictionary(
                role => role,
                role => userRolesDict.Count(kvp => kvp.Value.Contains(role))
            );

            // Pobierz kursy wraz z mentorem i zadaniami
            var courses = await _context.Courses
                .Include(c => c.Mentor)
                .Include(c => c.Tasks)
                .ToListAsync();

            // Przygotuj model kursów z managerem i liczbą tasków
            var courseModels = courses.Select(c =>
                new CourseReportModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    TaskCount = c.Tasks.Count,
                    ManagerName = c.Mentor != null ? $"{c.Mentor.Name} {c.Mentor.Surname}" : "Brak",
                    Tasks = null
                }).ToList();

            // Pobierz użytkowników o roli Nowy wraz z kursami, do których są przypisani
            var newRoleName = "Nowy";
            var newUserIds = userRolesDict.Where(kvp => kvp.Value.Contains(newRoleName)).Select(kvp => kvp.Key).ToList();

            var newUsersInCourses = await _context.UserCourses
                .Where(uc => newUserIds.Contains(uc.UserId))
                .Include(uc => uc.User)
                .Include(uc => uc.Course)
                .Select(uc => new NewUserCourseModel
                {
                    UserId = uc.UserId,
                    UserName = $"{uc.User.Name} {uc.User.Surname}",
                    CourseName = uc.Course.Name
                })
                .ToListAsync();

            var model = new StatisticReportViewModel
            {
                Roles = roles,
                UserCountsByRole = userCountsByRole,
                Courses = courseModels,
                NewUsersInCourses = newUsersInCourses
            };
            model.NewUsers = await _context.Users
            .Where(u => newUserIds.Contains(u.Id))
            .ToListAsync();

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> GetNewUsersByCourse(int courseId)
        {
            var newRole = "Nowy";
            var users = await _context.Users.ToListAsync();
            var userRolesDict = new Dictionary<int, IList<string>>();

            foreach (var user in users)
            {
                var rolesForUser = await _userManager.GetRolesAsync(user);
                userRolesDict[user.Id] = rolesForUser;
            }

            var newUserIds = userRolesDict.Where(kvp => kvp.Value.Contains(newRole)).Select(kvp => kvp.Key).ToList();

            var newUsersInCourse = await _context.UserCourses
                .Where(uc => uc.CourseId == courseId && newUserIds.Contains(uc.UserId))
                .Include(uc => uc.User)
                .Select(uc => new
                {
                    userId = uc.UserId,
                    userName = uc.User.Name + " " + uc.User.Surname
                })
                .ToListAsync();

            return Json(newUsersInCourse);
        }
        [HttpGet]
        public async Task<IActionResult> GetUsersByRole(string role)
        {
            var users = await _context.Users.ToListAsync();
            var userRolesDict = new Dictionary<int, IList<string>>();

            foreach (var user in users)
            {
                var rolesForUser = await _userManager.GetRolesAsync(user);
                userRolesDict[user.Id] = rolesForUser;
            }

            var filteredUsers = users
                .Where(u => userRolesDict.ContainsKey(u.Id) && userRolesDict[u.Id].Contains(role))
                .Select(u => new { u.Id, Name = $"{u.Name} {u.Surname}", u.Login })
                .ToList();

            return Json(filteredUsers);
        }

        [HttpGet]
        public async Task<IActionResult> GetDetailsByRoleUser(string role, int userId)
        {
            object result = null;

            switch (role)
            {
                case "Manager":
                    // Kursy przypisane managerowi
                    var courses = await _context.Courses
                        .Where(c => c.MentorId == userId)
                        .Select(c => new { c.Id, c.Name })
                        .ToListAsync();
                    result = courses;
                    break;

                case "Mentor":
                    // Taski mentora
                    var tasks = await _context.Tasks
                        .Where(t => t.MentorId == userId)
                        .Select(t => new { t.Id, t.Title })
                        .ToListAsync();
                    result = tasks;
                    break;

                case "Buddy":
                    // Podopieczni buddy
                    var buddies = await _context.Users
                        .Where(u => u.BuddyId == userId)
                        .Select(u => new { u.Id, Name = $"{u.Name} {u.Surname}" })
                        .ToListAsync();
                    result = buddies;
                    break;

                case "Nowy":
                    // Kursy przypisane nowemu użytkownikowi
                    var userCourses = await _context.UserCourses
                        .Where(uc => uc.UserId == userId)
                        .Include(uc => uc.Course)
                        .Select(uc => new { uc.Course.Id, uc.Course.Name })
                        .ToListAsync();
                    result = userCourses;
                    break;
            }

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetCourseTasks(int courseId)
        {
            var course = await _context.Courses
                .Include(c => c.Tasks)
                .ThenInclude(t => t.Mentor)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null) return Json(null);

            var tasks = course.Tasks.Select(t => new
            {
                t.Id,
                t.Title,
                MentorName = t.Mentor != null ? $"{t.Mentor.Name} {t.Mentor.Surname}" : "Brak"
            });

            return Json(tasks);
        }
        /*
        public async Task<IActionResult> GenerateNewUserReportPdf(int userId)
        {
            // Pobierz użytkownika z buddy i kursami
            var user = await _context.Users
                .Include(u => u.Buddy)
                .Include(u => u.UserCourses).ThenInclude(uc => uc.Course)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return NotFound();

            // Pobierz taski użytkownika osobno
            var userTasks = await _context.UserTasks
                .Include(ut => ut.Task)
                .Where(ut => ut.UserId == userId)
                .ToListAsync();

            // Pobierz wyniki testów użytkownika (UserId w UserTestResult to string)
            var userIdString = user.Id.ToString();
            var userTestResults = await _context.UserTestResults
                .Include(utr => utr.Test)
                .Where(utr => utr.UserId == userIdString)
                .ToListAsync();

            // Feedbacki pomijamy, bo brak klasy i DbSet

            var pdfBytes = GeneratePdfForNewUser(user, userTasks, userTestResults);

            return File(pdfBytes, "application/pdf", $"Raport_Nowy_{user.Name}_{user.Surname}.pdf");
        }

        private byte[] GeneratePdfForNewUser(User user, List<UserTask> userTasks, List<UserTestResult> userTestResults)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);
                    page.Header().Text($"Raport dla użytkownika {user.Name} {user.Surname}").FontSize(20).Bold();
                    page.Content().Stack(stack =>
                    {
                        stack.Item().Text($"Buddy: {(user.Buddy != null ? user.Buddy.Name + " " + user.Buddy.Surname : "Brak")}");

                        stack.Item().Text("Kursy przypisane:");
                        foreach (var uc in user.UserCourses)
                        {
                            stack.Item().Text($"- {uc.Course.Name}");
                        }

                        stack.Item().Text("Postęp w taskach:");
                        foreach (var ut in userTasks)
                        {
                            var statusText = ut.Status == StatusTask.Completed ? "ukończone" : "w trakcie";
                            var gradeText = !string.IsNullOrWhiteSpace(ut.Grade) ? $" | Feedback: {ut.Grade}" : "";
                            stack.Item().Text($"- {ut.Task.Title}: {statusText}{gradeText}");
                        }

                        stack.Item().Text("Postęp w testach:");
                        foreach (var utr in userTestResults)
                        {
                            stack.Item().Text($"- {utr.Test.Name}: poprawnych odpowiedzi {utr.CorrectAnswers}");
                        }
                    });
                });
            });

            return document.GeneratePdf();
        }*/
    }
}
