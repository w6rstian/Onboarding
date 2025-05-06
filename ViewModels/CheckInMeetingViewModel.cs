using Microsoft.AspNetCore.Mvc.Rendering;

namespace Onboarding.ViewModels
{
    public class CheckInMeetingViewModel
    {
        public int NewUserId { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public string? Title { get; set; }

        public List<SelectListItem> NewUsers { get; set; } = new();
    }
}
