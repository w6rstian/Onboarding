namespace Onboarding.Models
{
	/// <summary>
	/// This model holds recipient data for sending emails.
	/// </summary>
	public class EmailRequest
	{
		public string RecipientEmail { get; set; }
		public string Subject { get; set; }
		public string MessageBody { get; set; }
	}
}
