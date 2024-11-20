namespace Onboarding.Models
{
    public class CourseTask
    {
        public int Id { get; set; }
        public byte[] Upload { get; set; } 
        public bool isCompleted { get; set; } = false;
        public int TaskId { get; set; }
        public int UserCourseId { get; set; }

        
        public Task Task { get; set; }
        public CourseUser CourseUser { get; set; }

    }
}
