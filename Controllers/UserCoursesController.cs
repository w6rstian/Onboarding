using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Onboarding.Data;
using Onboarding.Models;

namespace Onboarding.Controllers
{
    public class UserCoursesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserCoursesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: UserCourses
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.UserCourses.Include(u => u.Course).Include(u => u.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: UserCourses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userCourse = await _context.UserCourses
                .Include(u => u.Course)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userCourse == null)
            {
                return NotFound();
            }

            return View(userCourse);
        }

        [Authorize(Roles = "Admin,HR")]
        // GET: UserCourses/Create
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name");

            var UsersNowy = (from user in _context.Users
                            join userRole in _context.UserRoles on user.Id equals userRole.UserId
                            join role in _context.Roles on userRole.RoleId equals role.Id
                            where role.Name == "Nowy"
                            select new
                            {
                                user.Id,
                                Name = user.Name + " " + user.Surname
                            }).ToList();

            ViewData["UserId"] = new SelectList(UsersNowy, "Id", "Name");
            return View();
        }

        // POST: UserCourses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int UserID, int CourseID)
        {
            var UserCourse = new UserCourse
            {
                UserId = UserID,
                CourseId = CourseID
            };
            if (ModelState.IsValid)
            {
                _context.Add(UserCourse);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name", UserCourse.CourseId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Name", UserCourse.UserId);
            return View(UserCourse);
        }

        // GET: UserCourses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userCourse = await _context.UserCourses.FindAsync(id);
            if (userCourse == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name", userCourse.CourseId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Name", userCourse.UserId);
            return View(userCourse);
        }

        // POST: UserCourses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,CourseId")] UserCourse userCourse)
        {
            if (id != userCourse.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userCourse);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserCourseExists(userCourse.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Id", userCourse.CourseId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", userCourse.UserId);
            return View(userCourse);
        }

        // GET: UserCourses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userCourse = await _context.UserCourses
                .Include(u => u.Course)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userCourse == null)
            {
                return NotFound();
            }

            return View(userCourse);
        }

        // POST: UserCourses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userCourse = await _context.UserCourses.FindAsync(id);
            if (userCourse != null)
            {
                _context.UserCourses.Remove(userCourse);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserCourseExists(int id)
        {
            return _context.UserCourses.Any(e => e.Id == id);
        }
    }
}
