using Microsoft.AspNetCore.Mvc;
using Onboarding.Data;
using Onboarding.Models;
using Microsoft.EntityFrameworkCore;

namespace Onboarding.Controllers
{
	public class NotificationsController : Controller
	{
		private readonly ApplicationDbContext _context;

		// Konstruktor z wstrzykiwaniem zależności
		public NotificationsController(ApplicationDbContext context)
		{
			_context = context;
		}

		// Wyświetlanie powiadomień użytkownika
		public async Task<IActionResult> Index()
		{
			// Załóżmy, że identyfikator użytkownika jest dostępny w HttpContext.User.Identity
			var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

			if (userId == null)
			{
				return Unauthorized();
			}

			var notifications = await _context.Notifications
				.Where(n => n.UserId == userId)
				.OrderByDescending(n => n.CreatedAt)
				.ToListAsync();

			return View(notifications);
		}

		// Tworzenie nowego powiadomienia
		[HttpPost]
		public async Task<IActionResult> Create(string userId, string message)
		{
			if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(message))
			{
				return BadRequest("User ID and message cannot be empty.");
			}

			var notification = new Notification
			{
				UserId = userId,
				Message = message,
				CreatedAt = DateTime.UtcNow,
				IsRead = false
			};

			_context.Notifications.Add(notification);
			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index));
		}

		// Oznaczanie powiadomienia jako przeczytane
		[HttpPost]
		public async Task<IActionResult> MarkAsRead(int id)
		{
			var notification = await _context.Notifications.FindAsync(id);
			if (notification == null)
			{
				return NotFound();
			}

			// Załóżmy, że identyfikator użytkownika to User.Identity.Name
			if (notification.UserId != User.Identity.Name)
			{
				return Forbid();
			}

			notification.IsRead = true;
			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index));
		}
	}
}
