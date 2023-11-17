using Microsoft.EntityFrameworkCore;

namespace CourseManagerApp.Entities
{
	public class CourseManagerDbContext : DbContext
	{
		public CourseManagerDbContext(DbContextOptions<CourseManagerDbContext> options) : base(options) { }

		// Database for Course
		public DbSet<Course> Courses { get; set; }

		//Database For Student
		public DbSet<Student> Students { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Student>()
				.Property(student => student.Status)
				.HasConversion<string>()
				.HasMaxLength(64);

			
			modelBuilder.Entity<Course>().HasData(
				new Course
				{
					CourseId = 1,
					CourseName = "Programing",
					CourseInstructor = "Bhargav",
					StartDate = new DateTime(2023 , 12, 31),
					RoomNumber = "2B14"
				});

			modelBuilder.Entity<Student>().HasData(
				new Student
				{
					StudentId= 1,
					StudentName = "Bhargav",
                    studentEmail = "bhargavpatel@gmail.com",
					CourseId = 1,

				});
		}
	}
}