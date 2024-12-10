using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Onboarding.Data;
using Onboarding.Models;

namespace Onboarding.Controllers
{
	public class NotificationsController : Controller
	{
		private readonly ApplicationDbContext _context;

		public NotificationsController(ApplicationDbContext context)
		{
			_context = context;
		}

		// GET: Notifications
		public async Task<IActionResult> Index()
		{
			// Pobierz Id zalogowanego użytkownika z claims
			int.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var userId);

			if (userId == 0)
			{
				return Unauthorized();
			}

			var notifications = await _context.Notifications
				.Where(n => n.UserId == userId)
				.OrderByDescending(n => n.CreatedAt)
				.ToListAsync();

			return View(notifications);
		}

		// GET: Notifications/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var notification = await _context.Notifications
				.FirstOrDefaultAsync(n => n.Id == id);

			if (notification == null)
			{
				return NotFound();
			}

			return View(notification);
		}

		// GET: Notifications/Create
		public IActionResult Create()
		{
			return View();
		}

		// POST: Notifications/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,UserId,Title,Message,CreatedAt")] Notification notification)
		{
			if (ModelState.IsValid)
			{
				notification.CreatedAt = DateTime.UtcNow; // Ustaw aktualną datę
				_context.Add(notification);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}

			return View(notification);
		}

		// GET: Notifications/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var notification = await _context.Notifications.FindAsync(id);

			if (notification == null)
			{
				return NotFound();
			}

			return View(notification);
		}

		// POST: Notifications/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,Title,Message,CreatedAt")] Notification notification)
		{
			if (id != notification.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(notification);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!NotificationExists(notification.Id))
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

			return View(notification);
		}

		// GET: Notifications/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var notification = await _context.Notifications
				.FirstOrDefaultAsync(n => n.Id == id);

			if (notification == null)
			{
				return NotFound();
			}

			return View(notification);
		}

		// POST: Notifications/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var notification = await _context.Notifications.FindAsync(id);

			if (notification != null)
			{
				_context.Notifications.Remove(notification);
				await _context.SaveChangesAsync();
			}

			return RedirectToAction(nameof(Index));
		}

		private bool NotificationExists(int id)
		{
			return _context.Notifications.Any(e => e.Id == id);
		}
	}
}
