using CourseManagerApp.Entities;
namespace CourseManagerApp.Models
{
    public class ManageCourseViewModel
    {
        public Course? Course { get; set; }
        public Student? Student { get; set; }
        public int CountConfirmationMessageNotSent  { get; set; }
        public int CountConfirmationMessageSent { get; set; }
        public int CountEnrollmentConfirm { get; set; }
        public int CountEnrollmentDecline { get; set; }

        
    }
}
