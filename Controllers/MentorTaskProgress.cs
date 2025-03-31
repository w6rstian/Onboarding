using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Onboarding.Data;
using Onboarding.Models;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

public class MentorTaskProgress : Controller
{
    private readonly ApplicationDbContext _context;

    public MentorTaskProgress(ApplicationDbContext context)
    {
        _context = context;
    }

    // 1. Lista tasków, gdzie użytkownik jest mentorem
    public async Task<IActionResult> Index()
    {
        var currentUserId = GetCurrentUserId();

        var tasks = await _context.Tasks
            .Include(t => t.Course)
            .Where(t => t.MentorId == currentUserId)
            .ToListAsync();

        return View(tasks);
    }

    // 2. Szczegóły - progres uczestników danego taska
    public async Task<IActionResult> Details(int id)
    {
        var task = await _context.Tasks
            .Include(t => t.Course)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task == null)
            return NotFound();

        var userTasks = await _context.UserTasks
            .Include(ut => ut.user)
            .Where(ut => ut.TaskId == id)
            .ToListAsync();

        var progressList = userTasks.Select(ut =>
        {
            var steps = JsonSerializer.Deserialize<List<UserTaskStep>>(ut.UserTaskStepsJson ?? "[]");
            var completedSteps = steps?.Count(s => s.IsCompleted) ?? 0;
            var totalSteps = steps?.Count ?? 0;

            return new
            {
                User = ut.user,
                CompletedSteps = completedSteps,
                TotalSteps = totalSteps,
                Status = ut.Status
            };
        }).ToList();

        ViewBag.TaskTitle = task.Title;

        return View(progressList);
    }

    private int GetCurrentUserId()
    {
        return int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
    }
}
