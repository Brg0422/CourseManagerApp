using System.ComponentModel.DataAnnotations;

namespace CourseManagerApp.Entities
{
	public class Course
	{
		public int CourseId { get; set; }

		[Required(ErrorMessage ="What's your course? ")]
		public string? CourseName { get; set; }

		[Required(ErrorMessage = "Who is the instructor? ")]
		public string? CourseInstructor { get; set; }


		[Required(ErrorMessage = "When is your course? ")]
		public DateTime? StartDate { get; set; }


		[Required(ErrorMessage = "Where is the course? ")]
		[RegularExpression(@"^\d[A-Z]\d{2}$",ErrorMessage="Must bein the formate 1X11")]
		public string? RoomNumber { get; set; }

		public List<Student>? Students { get; set; }


	}
}
