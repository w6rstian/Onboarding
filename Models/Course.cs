using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CompanyId { get; set; }

        public Company Company { get; set; }
        public ICollection<UserCourse> UserCourses { get; set; }
        public ICollection<Task> Tasks { get; set; }
        public ICollection<Test> Tests { get; set; } //nowe
    }
}
