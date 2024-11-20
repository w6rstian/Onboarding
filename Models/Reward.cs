using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Onboarding.Models
{
    public class Reward
    {
        [Key]
        public int ID { get; set; }

        [ForeignKey("ReceiverId")]
        public User Receiver { get; set; } 
        public int ReceiverId { get; set; } 

        [ForeignKey("GiverId")]
        public User Giver { get; set; }
        public int GiverId { get; set; } 

        [Range(1, 10)]
        public int Rating { get; set; }

        public string Feedback { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
    }

}
