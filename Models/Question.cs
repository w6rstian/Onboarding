namespace Onboarding.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Net;
    using static System.Net.Mime.MediaTypeNames;

    public class Question
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; }

        public List<string> Answers { get; set; } = new List<string>();

        [MaxLength(200)]
        public string CorrectAnswer { get; set; }

        [ForeignKey("Test")]
        public int TestId { get; set; }
        public Test Test { get; set; }
    }

}
