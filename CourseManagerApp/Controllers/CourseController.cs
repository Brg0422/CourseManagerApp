using CourseManagerApp.Models;
using CourseManagerApp.Services;
using Microsoft.AspNetCore.Mvc;
using CourseManagerApp.Entities;
using System.Reflection.Metadata.Ecma335;

namespace CourseManagerApp.Controllers
{
	public class CourseController : AbstractBaseController
	{
		private readonly ICourseManagerService _courseManagerService;

		public CourseController(ICourseManagerService courseManagerService)
		{
			_courseManagerService = courseManagerService;
		}

		[HttpGet("/Courses")]
		public IActionResult List() 
		{
			SetWelcome();
			var coursesViewModel = new CoursesViewModel()
			{
				Courses = _courseManagerService.GetAllCourses(),
			};
			return View(coursesViewModel);
		}

		[HttpGet("/Courses/{id:int}")]
		public IActionResult Manage(int id)
		{
			SetWelcome();
			var course = _courseManagerService.GetCourseById(id);

			if(course == null)
			{
				return NotFound();
			}

			var manageCourseViewModel = new ManageCourseViewModel() 
			{
				Course = course,
				Student = new Student(),
				CountConfirmationMessageNotSent = course.Students.Count(g=>g.Status == EnrollementConformationStatus.ConfirmationMessageNotSent),
				CountConfirmationMessageSent = course.Students.Count(g => g.Status == EnrollementConformationStatus.ConfirmationMessageSent),
				CountEnrollmentConfirm = course.Students.Count(g => g.Status == EnrollementConformationStatus.EnrollmentConfirm),
				CountEnrollmentDecline = course.Students.Count(g => g.Status == EnrollementConformationStatus.EnrollmentDecline),
			};
			return View(manageCourseViewModel);
		}

		[HttpGet("/courses/add")]
		public IActionResult Add()
		{
			SetWelcome();
            var courseViewModel = new CourseViewModel()
            {
                Course = new Course()
            };
            return View(courseViewModel);
		}

        [HttpPost("/courses/add")]

        public IActionResult Add(CourseViewModel courseViewModel)
        {
            SetWelcome();
           if(!ModelState.IsValid) { 
			return View(courseViewModel);
			}
			
		   _courseManagerService.AddCourse(courseViewModel.Course);
			TempData["notify"] = $"{courseViewModel.Course.CourseName} Added Successfully!!";
			TempData["class"] = "success";
			return RedirectToAction("Manage", new { id = courseViewModel.Course.CourseId });
            
        }


        [HttpGet("/Courses/{id:int}/edit")]
		public IActionResult Edit(int id)
		{
			SetWelcome();

            var course = _courseManagerService.GetCourseById(id);

            if (course == null)
            {
                return NotFound();
            }

			var courseViewModel = new CourseViewModel()
			{
                Course = course
			};
			return View(courseViewModel);
		}

        [HttpPost("/Courses/{id:int}/edit")]
        public IActionResult Edit(int id, CourseViewModel courseViewModel)
        {
            SetWelcome();

			if (!ModelState.IsValid)
			{
				return View(courseViewModel);
			}

            _courseManagerService.UpdateCourse(courseViewModel.Course);
            TempData["notify"] = $"{courseViewModel.Course.CourseName} Updated Successfully!!";
            TempData["class"] = "info";
            return RedirectToAction("Manage", new { id });

        }


		[HttpGet("/thankyou/{response}")]
		public IActionResult ThankYou(string response) {

			SetWelcome();
			return View("ThankYou", response);
		
		}


		[HttpGet("/courses/{courseId:int}/enroll/{studentId:int}")]
		public IActionResult Enroll(int courseId, int studentId)
		{
			SetWelcome();

			var student = _courseManagerService.GetStudentById(courseId,studentId) ;

			if(student==null)return NotFound();

			var enrollStudentViewModel = new EnrollStudentViewModel()
			{
				Student = student
			};
			return View(enrollStudentViewModel);
		}


        [HttpPost("/courses/{courseId:int}/enroll/{studentId:int}")]
        public IActionResult Enroll(int courseId, int studentId, EnrollStudentViewModel enrollStudentViewModel)
        {
            SetWelcome();

            var student = _courseManagerService.GetStudentById(courseId, studentId);

            if (student == null) return NotFound();

			if (ModelState.IsValid)
			{
				var status = enrollStudentViewModel.Response == "Yes" ?
					EnrollementConformationStatus.EnrollmentConfirm : EnrollementConformationStatus.EnrollmentDecline;
				_courseManagerService.UpdateConfirmationStatus(courseId, studentId, status);

				return RedirectToAction("ThankYou", new { response = enrollStudentViewModel.Response});
			}
			else
			{

				enrollStudentViewModel.Student = student;
				return View(enrollStudentViewModel);
			}
		
           
           
        }



		[HttpPost("/course/{courseId:int}/add-student")]
		public IActionResult AddStudent(int courseId, ManageCourseViewModel manageCourseViewModel)
		{
			SetWelcome();
			Course? course;

			if(ModelState.IsValid)
			{
                _courseManagerService.AddStudentByCourseId(courseId,manageCourseViewModel.Student);
                TempData["notify"] = $"{manageCourseViewModel?.Student?.StudentName} Added Successfully!!";
                TempData["class"] = "success";
               

                return RedirectToAction("Manage", new { id = courseId });
			}
			else
			{
				course = _courseManagerService.GetCourseById(courseId);
				
				if (course == null) return NotFound();
				manageCourseViewModel.Course = course;
                manageCourseViewModel.CountConfirmationMessageNotSent = course.Students.Count(g => g.Status == EnrollementConformationStatus.ConfirmationMessageNotSent);
                manageCourseViewModel.CountConfirmationMessageSent = course.Students.Count(g => g.Status == EnrollementConformationStatus.ConfirmationMessageSent);
                manageCourseViewModel.CountEnrollmentConfirm = course.Students.Count(g => g.Status == EnrollementConformationStatus.EnrollmentConfirm);
                manageCourseViewModel.CountEnrollmentDecline = course.Students.Count(g => g.Status == EnrollementConformationStatus.EnrollmentDecline);

                return View("Manage", manageCourseViewModel);
			}
		}



		[HttpPost("/course/{courseId:int}/enroll")]
		public IActionResult SendInvitation(int courseId)
		{
			_courseManagerService.SendEnrollmentEmailByCourseId(courseId, Request.Scheme, Request.Host.ToString());

			return RedirectToAction("Manage", new {id=courseId});
		}

    }
}
