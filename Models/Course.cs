using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Onboarding.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        
        [ForeignKey("Company")]
        public int CompanyID { get; set; } 
        public Company Company { get; set; }

        public List<User> Users { get; set; } = new List<User>();
        public List<Task> Tasks { get; set; } = new List<Task>();
        //public List<CourseUser> CourseUsers { get; set; } = new List<CourseUser>(); //new
    }
}
