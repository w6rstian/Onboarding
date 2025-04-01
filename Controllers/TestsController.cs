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
    public class TestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin,Manager")]
        // GET: Tests
        public async Task<IActionResult> Index()
        {
            var tests = await _context.Tests
                                         .Include(t => t.Course) // Include Course so it is loaded with the Test
                                         .ToListAsync();
            return View(tests);

        }


        // GET: Tests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var test = await _context.Tests
                .Include(t => t.Course)
        .Include(t => t.Task)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (test == null)
            {
                return NotFound();
            }
            ViewBag.Tasks = test.Task != null ? new List<string> { test.Task.Title } : null;


            return View(test);
        }

        // GET: Tests/Create
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name");
            ViewData["TaskId"] = new SelectList(_context.Tasks, "Id", "Title");
            return View();
        }

        // POST: Tests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string Name,int TaskId,int CourseId)
        {
            if (string.IsNullOrEmpty(Name) || CourseId == 0)
            {
                ModelState.AddModelError(string.Empty, "Name and CourseId are required.");
            }
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.Id == CourseId);

            if (course == null)
            {
                return NotFound();
            }
            var test = new Test
            {
                Name = Name,
                TaskId = TaskId,
                Course = course,
                Questions = new List<Question>()
            };
            Console.WriteLine("Test: "+ test);
            if (ModelState.IsValid)
            {
                
                _context.Add(test);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Field: {state.Key}, Error: {error.ErrorMessage}");
                    }
                }
            }
            ViewBag.CourseId = new SelectList(_context.Courses, "Id", "Name", test.Course?.Id);
            return View(test);
            //return RedirectToAction(nameof(Index));
        }

        // GET: Tests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var test = await _context.Tests
                .Include(t => t.Course) // Load the associated Course
                .Include(t => t.Task)   // Load the associated Task
                .FirstOrDefaultAsync(t => t.Id == id);

            if (test == null)
            {
                return NotFound();
            }

            // Populate ViewBag with data for dropdowns
            ViewBag.Tasks = new SelectList(_context.Tasks, "Id", "Title", test.TaskId);

            return View(test);
        }

        // POST: Tests/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,TaskId")] Test test)
        {
            if (id != test.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // We do not change the Course, only update the TaskId and Name.
                    var existingTest = await _context.Tests
                        .Include(t => t.Course) // Make sure to load the course, but do not change it
                        .FirstOrDefaultAsync(t => t.Id == test.Id);

                    if (existingTest == null)
                    {
                        return NotFound();
                    }

                    existingTest.Name = test.Name; // Update the Name
                    existingTest.TaskId = test.TaskId; // Update the TaskId

                    _context.Update(existingTest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TestExists(test.Id))
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
            return View(test);
        }



        // GET: Tests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var test = await _context.Tests
                .Include(t => t.Task)
                 .Include(t => t.Course)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (test == null)
            {
                return NotFound();
            }

            ViewBag.Tasks = test.Task; 
            return View(test);
        }



        // POST: Tests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var test = await _context.Tests.FindAsync(id);
            if (test != null)
            {
                _context.Tests.Remove(test);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TestExists(int id)
        {
            return _context.Tests.Any(e => e.Id == id);
        }
    }
}
