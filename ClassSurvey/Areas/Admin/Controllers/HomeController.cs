using ClassSurvey.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClassSurvey.Controllers
{
	public class HomeController : Controller
	{
		private ClassSurveyDbContext db = new ClassSurveyDbContext();
		public ActionResult Index()
		{
			if(Session["User"] != null)
			{
				ViewBag.CountClasses = db.Classes.ToList().Count();
				ViewBag.CountStudents = db.Students.ToList().Count();
				ViewBag.CountSurveys = db.Surveys.GroupBy(x => x.StudentClassId).Count();
				return View();
			}
			return RedirectToAction("Home", new { area = "Authentication" });
		}
	}
}