using MimeKit;
using MailKit.Net.Smtp;

namespace Onboarding.Services
{
	/// <summary>
	/// This service sends emails asynchronically using email server configuration and authentication credentials from appsettings.json.
	/// </summary>
	// MAKE SURE TO UPDATE APPSETTINGS.JSON WITH VALID CREDENTIALS FOR EMAIL AUTHENTICATION
	public class EmailService
	{
		/*
		// USING APPSETTINGS.JSON INSTEAD v-v
		private const string SERVER_ADDRESS = "smtp.example.com";
		private const string SENDER_EMAIL = "your-email@example.com";
		private const string SENDER_PASSWORD = "your-email-password";
		private const string SENDER_NAME = "John Pork";

		private readonly string _smtpServer = SERVER_ADDRESS;
		private readonly int _port = 587;
		private readonly string _senderEmail = SENDER_EMAIL;
		private readonly string _senderPassword = SENDER_PASSWORD;
		*/

		private readonly IConfiguration _configuration;

		public EmailService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public async Task SendEmailAsync(string recipientEmail, string subject, string messageBody)
		{
			var emailMessage = new MimeMessage();

			emailMessage.From.Add(new MailboxAddress(
				_configuration["EmailCredentials:SenderName"],
				_configuration["EmailCredentials:SenderEmail"]
				));
			emailMessage.To.Add(new MailboxAddress(
				recipientEmail.Substring(0, recipientEmail.IndexOf('@') + 1),
				recipientEmail
			));

			emailMessage.Subject = subject;
			emailMessage.Body = new TextPart("plain") { Text = messageBody };

			using (var smtpClient = new SmtpClient())
			{
				try
				{
					await smtpClient.ConnectAsync(
						_configuration["EmailCredentials:SmtpServer"],
						int.Parse(_configuration["EmailCredentials:Port"]),
						MailKit.Security.SecureSocketOptions.StartTls
					);

					await smtpClient.AuthenticateAsync(
						_configuration["EmailCredentials:SenderEmail"],
						_configuration["EmailCredentials:SenderPassword"]
					);

					await smtpClient.SendAsync(emailMessage);
				}
				finally
				{
					await smtpClient.DisconnectAsync(true);
				}
			}
		}
	}
}
