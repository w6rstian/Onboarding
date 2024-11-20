using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Onboarding.Models
{
    public class Article
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Content { get; set; }
        
        [ForeignKey("Task")]
        public int TaskId { get; set; }
        public Task Task { get; set; }
    }
}
