using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Onboarding.Models
{
    public class CourseUser
    {
        public int Id { get; set; }

        [ForeignKey("Course")]
        public int CourseId { get; set; }
        public Course Course { get; set; }

        //[ForeignKey("User")]
        
        public int UserId { get; set; }
        public User User { get; set; }

        public int? TestResult { get; set; }

        //public List<CourseTask> CourseTasks { get; set; } = new List<CourseTask>();
    }
}
