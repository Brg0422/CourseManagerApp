using System.ComponentModel.DataAnnotations;

namespace CourseManagerApp.Entities
{
	public enum EnrollementConformationStatus 
	{ 
		ConfirmationMessageNotSent = 0, 
		ConfirmationMessageSent = 1 , 
		EnrollmentConfirm = 2, 
		EnrollmentDecline = 3
	}
	public class Student
	{
		public int StudentId { get; set; }

		[Required(ErrorMessage = "Who is the student for today's course? ")]
		public string? StudentName { get; set; }

		[Required(ErrorMessage = "What is the student email? ")]
		[EmailAddress(ErrorMessage ="Must Be Valid Email-Address")]
		public string? studentEmail { get; set; }

		public EnrollementConformationStatus Status { get; set; } = EnrollementConformationStatus.ConfirmationMessageNotSent;

		// Status

		public int CourseId { get; set; }

		public Course? Course { get; set; }
	}
}
