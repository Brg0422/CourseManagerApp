using CourseManagerApp.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace CourseManagerApp.Services
{
    public class CourseManagerService : ICourseManagerService
    {
        private readonly IConfiguration _configuration;
        private readonly CourseManagerDbContext _courseManagerDbContext;

        public CourseManagerService(CourseManagerDbContext courseManagerDbContext, IConfiguration configuration)
        {
            _courseManagerDbContext = courseManagerDbContext;
            _configuration = configuration;
        }

        public List<Course> GetAllCourses()
        {
            return _courseManagerDbContext.Courses.Include(e => e.Students).OrderByDescending(e => e.StartDate).ToList();
        }

        // For the manage Page by getting a course by the id
        public Course? GetCourseById(int id)
        {
            return _courseManagerDbContext.Courses.Include(e => e.Students).FirstOrDefault(e => e.CourseId == id);
        }

        public Student? GetGuestById(int CourseId, int Studentid)
        {
            return _courseManagerDbContext.Students
                .Include(g => g.Course)
                .FirstOrDefault(g => g.CourseId == CourseId && g.StudentId == Studentid);
        }

        public void UpdateConfirmationStatus(int courseId, int studentId, EnrollementConformationStatus status)
        {
            var student = GetStudentById(courseId, studentId);

            if (student == null) { return; }

            student.Status = status;

            _courseManagerDbContext.SaveChanges();
        }

        public int AddCourse(Course Party)
        {
            _courseManagerDbContext.Courses.Add(Party);
            _courseManagerDbContext.SaveChanges();

            return Party.CourseId;
        }

        public void UpdateCourse(Course Party)
        {
            _courseManagerDbContext.Courses.Update(Party);
            _courseManagerDbContext.SaveChanges();
        }

        public Course? AddStudentByCourseId(int CourseId, Student? Student)
        {
            var party = GetCourseById(CourseId);

            if (party == null) { return null; }

            party.Students?.Add(Student);

            _courseManagerDbContext.SaveChanges();

            return party;
        }

        public void SendEnrollmentEmailByCourseId(int courseId, string scheme, string instructor)
        {
            var party = GetCourseById(courseId);

            if (party == null) { return; }

            var students = party?.Students?.Where(g => g.Status == EnrollementConformationStatus.ConfirmationMessageNotSent).ToList();

            try
            {
                var smtpInstructor = _configuration["SmtpSettings:Instructor"];
                var smtpPort = _configuration["SmtpSettings:Port"];
                var fromAddress = _configuration["SmtpSettings:FromAddress"];
                var fromPassword = _configuration["SmtpSettings:FromPassword"];

                using var smtpClient = new SmtpClient(smtpInstructor);
                smtpClient.Port = string.IsNullOrEmpty(smtpPort) ? 587 : Convert.ToInt32(smtpPort);

                smtpClient.Credentials = new NetworkCredential(fromAddress, fromPassword);
                smtpClient.EnableSsl = true;

                foreach (var student in students)
                {
                    var responseUrl = $"{scheme}://{instructor}/courses/{courseId}/enroll/{student.StudentId}";

                    var mailMessage = new MailMessage()
                    {
                        From = new MailAddress(fromAddress),
                        Subject = $"[Action Required] Confirm Enrollment for {student.StudentName}",
                        Body = CreateBody(party, responseUrl),
                        IsBodyHtml = true,
                    };

                    if (student.studentEmail == null) return;

                    mailMessage.To.Add(student.studentEmail);

                    smtpClient.Send(mailMessage);

                    student.Status = EnrollementConformationStatus.ConfirmationMessageSent;
                }

                _courseManagerDbContext.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static string CreateBody(Course course, string url)
        {
            return $@"
                <h1>Hello:</h1>
                <p>
                    Your request to enroll in the Course {course.CourseName}
                    in the room {course.RoomNumber}
                    starting on {course.StartDate:d}
                    with instructor {course.CourseInstructor}	
                </p>

                <p>
                    <a href={url}>Please click here to confirm your enrollment</a>
                </p>

                <p>
                    Sincerely,	
                </p>
                <p>
                    Course Manager App	
                </p>
            ";
        }

        public Student? GetStudentById(int CourseId, int Studentid)
        {
            return _courseManagerDbContext.Students
                .Include(s => s.Course)
                .FirstOrDefault(s => s.CourseId == CourseId && s.StudentId == Studentid);
        }
    }
}