using CourseManagerApp.Entities;

namespace CourseManagerApp.Services
{
	public interface ICourseManagerService
	{
		public List<Course> GetAllCourses();

		// FOr the manage Page by getting an course by the id 
		public Course? GetCourseById(int id);

		//
		public int AddCourse(Course Party);

		// 
		public void UpdateCourse(Course Party);


		public Student? GetStudentById(int CourseId,int Studentid);


		public Course? AddStudentByCourseId(int CourseId, Student? Student);

		public void SendEnrollmentEmailByCourseId(int courseId, string scheme, string instructor);


		public void UpdateConfirmationStatus(int courseId, int studentId, EnrollementConformationStatus status);
	}
}
