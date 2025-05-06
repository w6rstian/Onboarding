namespace Onboarding.Models
{
    public class CheckInMeeting
    {
        public int Id { get; set; }
        public int BuddyId { get; set; }
        //public User Buddy { get; set; }

        public int NewUserId { get; set; }
        public User NewUser { get; set; }

        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public string? Title { get; set; }
    }
}
