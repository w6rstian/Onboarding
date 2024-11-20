using System.ComponentModel.DataAnnotations;

namespace Onboarding.Models
{
    public class Company
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(11)]
        public string PhoneNumber { get; set; }

        [MaxLength(30)]
        public string City { get; set; }

        [MaxLength(6)]
        public string PostCode { get; set; }

        [MaxLength(50)]
        public string Street { get; set; }

        public int? StreetNumber { get; set; }

        [MaxLength(50)]
        public string Country { get; set; }

        public List <User> Users { get; set; } = new List<User>();
        public List <Course> Courses { get; set; } = new List<Course>();
    }
}
