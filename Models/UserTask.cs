namespace Onboarding.Models
{
	public class UserTask
	{
		public int UserTaskId { get; set; }

		public int TaskId { get; set; }
		public Task Task { get; set; }

		public int UserId { get; set; }
		public User user { get; set; }

		public StatusTask Status { get; set; }

		// Indywidualne kroki użytkownika przechowywane w JSON
		public string UserTaskStepsJson { get; set; }
	}

}
