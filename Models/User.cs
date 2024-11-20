using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Onboarding.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(1019)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string Surname { get; set; }

        [Required]
        [MaxLength(100)]
        public string Login { get; set; }

        [Required]
        [MaxLength(200)]
        public string Password { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public int? BuddyId { get; set; }

        [ForeignKey("Company")]
        public int CompanyId { get; set; }

        public Company Company { get; set; }
        public User Buddy { get; set; }

        [InverseProperty("Giver")]
        public List<Reward> GivenRewards { get; set; } = new List<Reward>();

        [InverseProperty("Receiver")]
        public List<Reward> ReceivedRewards { get; set; } = new List<Reward>();

        public List<Course> Courses { get; set; } = new List<Course>();
        public List<Task> Tasks { get; set; } = new List<Task>();
        
        //public List<CourseUser> CourseUsers { get; set; } = new List<CourseUser>(); 
    }
}
