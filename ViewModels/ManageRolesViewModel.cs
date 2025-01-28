using Onboarding.Models;

namespace Onboarding.ViewModels
{
    public class ManageRolesViewModel
    {
        public User User { get; set; }
        public IList<string> AvailableRoles { get; set; }  // Change to IList<string>
        public IList<string> UserRoles { get; set; }
    }

}
