using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Onboarding.Data;
using Onboarding.Models;
using Task = Onboarding.Models.Task;

namespace Onboarding.Controllers
{
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tasks
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Tasks
                .Include(t => t.Course)
                .Include(t => t.Mentor)
                .Include(t => t.Articles)
                .Include(t => t.Links);
            return View(await applicationDbContext.ToListAsync());
        }   

        // GET: Tasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .Include(t => t.Course)
                .Include(t => t.Mentor)
                .Include(t => t.Articles)
                .Include(t => t.Links)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // GET: Tasks/Create
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Id");
            ViewData["MentorId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Tasks/Create.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int MentorId, string Title, string Description, int CourseId, string ArticleContent, string Links)
        {
            if (string.IsNullOrEmpty(Title) || string.IsNullOrEmpty(Description) || MentorId == 0 || CourseId == 0)
            {
                ModelState.AddModelError(string.Empty, "All fields are required.");
            }

            if (!ModelState.IsValid)    
            {
                ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Id");
                ViewData["MentorId"] = new SelectList(_context.Users, "Id", "Id");
                return View();
            }

            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.Id == CourseId);

            if (course == null)
            {
                return NotFound();
            }

            var mentor = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == MentorId);

            if (mentor == null)
            {
                return NotFound();
            }

            var task = new Task
            {
                MentorId = MentorId,
                Title = Title,
                Description = Description,
                CourseId = CourseId,
                Mentor = mentor,  
                Course = course   
            };

            if (!string.IsNullOrEmpty(Links))
            {
                string[] links = Links.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                foreach (var urlLink in links)
                {
                    var link = new Link
                    {
                        LinkUrl = urlLink,
                        Name = urlLink,
                        TaskId = task.Id,
                        Task = task
                    };

                    task.Links.Add(link);
                }
            }


            if (!string.IsNullOrEmpty(ArticleContent))
            {
                var article = new Article
                {
                    Content = ArticleContent,
                    Task = task
                };
                task.Articles.Add(article);
            }

            course.Tasks.Add(task);
            _context.Add(task);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .Include(t => t.Articles)
                .Include(t => t.Links)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Id", task.CourseId);
            ViewData["MentorId"] = new SelectList(_context.Users, "Id", "Id", task.MentorId);

            ViewBag.ArticleContent = task.Articles.FirstOrDefault()?.Content;
            ViewBag.Links = string.Join(" ", task.Links.Select(l => l.LinkUrl));

            return View(task);
        }

        // POST: Tasks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int MentorId, string Title, string Description, int CourseId, string ArticleContent, string Links)
        {
            var existingTask = await _context.Tasks
                .Include(t => t.Articles)
                .Include(t => t.Links)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (existingTask == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    existingTask.MentorId = MentorId;
                    existingTask.Title = Title;
                    existingTask.Description = Description;
                    existingTask.CourseId = CourseId;

                    var existingArticle = existingTask.Articles.FirstOrDefault();
                    if (!string.IsNullOrEmpty(ArticleContent))
                    {
                        if (existingArticle != null)
                        {
                            existingArticle.Content = ArticleContent;
                        }
                        else
                        {
                            existingTask.Articles.Add(new Article { Content = ArticleContent });
                        }
                    }
                    else if (existingArticle != null)
                    {
                        _context.Articles.Remove(existingArticle);
                    }

                    if (!string.IsNullOrEmpty(Links))
                    {
                        var links = Links.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        var existingLinks = existingTask.Links.ToList();

                        foreach (var existingLink in existingLinks)
                        {
                            if (!links.Contains(existingLink.LinkUrl))
                            {
                                _context.Links.Remove(existingLink);
                            }
                        }

                        foreach (var linkUrl in links)
                        {
                            if (!existingLinks.Any(l => l.LinkUrl == linkUrl))
                            {
                                existingTask.Links.Add(new Link { LinkUrl = linkUrl, Name = linkUrl });
                            }
                        }
                    }
                    else
                    {
                        foreach (var existingLink in existingTask.Links.ToList())
                        {
                            _context.Links.Remove(existingLink);
                        }
                    }

                    _context.Update(existingTask);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskExists(id))
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

            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Id", CourseId);
            ViewData["MentorId"] = new SelectList(_context.Users, "Id", "Id", MentorId);
            ViewBag.ArticleContent = ArticleContent;
            ViewBag.Links = Links;

            return View(existingTask);
        }


        // GET: Tasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .Include(t => t.Course)
                .Include(t => t.Mentor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaskExists(int id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }
    }
}
