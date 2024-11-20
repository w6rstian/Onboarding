using Microsoft.AspNetCore.Components.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Onboarding.Models
{
    public class Task
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        
        public string Description { get; set; }

        public int MentorId { get; set; }      

        public int CourseId { get; set; }
        public Course Course { get; set; }

        public List<Article> Articles { get; set; } = new List<Article>();
        public List<Link> Links { get; set; } = new List<Link>();
        public List<User> Users { get; set; } = new List<User>();
    }
}
