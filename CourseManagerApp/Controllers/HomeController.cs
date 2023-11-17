using CourseManagerApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CourseManagerApp.Controllers
{
	public class HomeController : AbstractBaseController
	{
		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
		}

		public IActionResult Index()
		{
			SetWelcome();
			return View();
		}

		public IActionResult Privacy()
		{
			SetWelcome();
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}