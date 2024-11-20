using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Onboarding.Models
{
    public class TaskUser
    {
        [Key]
        public int Id { get; set; } 

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        [ForeignKey("Task")]
        public int TaskId { get; set; }
        public Task Task { get; set; }

        public bool IsCompleted { get; set; } = false; 
    }
}
