namespace Onboarding.Models
{
	public class Notification
	{
		public int Id { get; set; }
		public int UserId { get; set; } // Id użytkownika, któremu powiadomienie jest przypisane
		public string Message { get; set; }
		public DateTime CreatedAt { get; set; }
		public bool IsRead { get; set; } // Czy użytkownik przeczytał powiadomienie

		// Relacja do użytkownika
		public User User { get; set; }
	}

}
