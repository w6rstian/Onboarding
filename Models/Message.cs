namespace Onboarding.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Content { get; set; }

        [Required]
        public DateTime SentAt { get; set; }

        [ForeignKey("SenderId")]
        public User Sender { get; set; }
        public int SenderId { get; set; }

        [ForeignKey("ReceiverId")]
        public User Receiver { get; set; }
        public int ReceiverId { get; set; }
    }

}
