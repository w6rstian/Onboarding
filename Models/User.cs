using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Role { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public int? BuddyId { get; set; }
        public int CompanyId { get; set; }

        public Company Company { get; set; }
        public User Buddy { get; set; }
        public ICollection<UserCourse> UserCourses { get; set; }
        public ICollection<Message> SentMessages { get; set; }
        public ICollection<Message> ReceivedMessages { get; set; }
        public ICollection<Reward> GivenRewards { get; set; }
        public ICollection<Reward> ReceivedRewards { get; set; }
        public ICollection<Announcement> Announcements { get; set; }
		public ICollection<Notification> Notifications { get; set; }
	}
}
