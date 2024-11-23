using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public string Country { get; set; }

        public ICollection<User> Users { get; set; }
        public ICollection<Course> Courses { get; set; }
    }
}
