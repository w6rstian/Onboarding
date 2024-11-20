using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Onboarding.Models
{
    public class Announcement
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime PublishDate { get; set; }

        [ForeignKey("User")]
        public int CreatedBy { get; set; }
        public User User { get; set; }
    }
}
