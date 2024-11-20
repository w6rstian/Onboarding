using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Onboarding.Models
{
    public class Link
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Url { get; set; }
        public string Name { get; set; }
        
        [ForeignKey("Task")]
        public int TaskId { get; set; }
        public Task Task { get; set; }
    }
}
