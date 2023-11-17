using Microsoft.AspNetCore.Mvc;

namespace CourseManagerApp.Controllers
{
	public abstract class AbstractBaseController : Controller
	{ 
		public void SetWelcome()
		{
			
			const string cookieName = "firstVisitDate";

			var firstVistDate = HttpContext.Request.Cookies.ContainsKey(cookieName) &&
				DateTime.TryParse(HttpContext.Request.Cookies[cookieName],out var parsedDate)
				? parsedDate
				: DateTime.Now;

			var welcomeMessage = HttpContext.Request.Cookies.ContainsKey(cookieName)
				? $"Welcme back! You first used this app on{firstVistDate.ToShortDateString()}" :
				"Hey, Welcome to the Course Manger App";

			var cookieOptions = new CookieOptions
			{
				Expires = DateTime.Now.AddDays(30),
			};

			HttpContext.Response.Cookies.Append(cookieName,
				firstVistDate.ToString(),
				cookieOptions);

			ViewData["WelcomeMessage"] = welcomeMessage;
		}
		
	}
}
