using Microsoft.AspNetCore.Mvc;
using Onboarding.Models;
using Onboarding.Services;

namespace Onboarding.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class EmailController : Controller
	{
		private readonly EmailService _emailService;

		public EmailController(EmailService emailService)
		{
			_emailService = emailService;
		}

		[HttpPost("send")]
		public async Task<IActionResult> SendEmail([FromBody] EmailRequest request)
		{
			try
			{
				await _emailService.SendEmailAsync(request.RecipientEmail, request.Subject, request.MessageBody);
				return Ok("Email sent successfully.");
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error sending email: {ex.Message}");
			}
		}
	}
}
