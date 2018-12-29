using ClassSurvey.Authorization;
using ClassSurvey.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClassSurvey.Areas.Admin.Controllers
{
	[AuthorizeBusiness]
	public class HomeController : Controller
	{
		//get DB
		private ClassSurveyDbContext db = new ClassSurveyDbContext();
		public ActionResult Index()
		{
			if(Session["User"] != null)
			{
				//get number of classes
				ViewBag.CountClasses = db.Classes.ToList().Count();
				//get number of students
				ViewBag.CountStudents = db.Students.ToList().Count();
				//get number of surveys
				ViewBag.CountSurveys = db.Surveys.GroupBy(x => x.StudentClassId).Count();
				//get number of teachers
				ViewBag.CountTeachers = db.Teachers.ToList().Count();
				return View();
			}
			return RedirectToAction("Home", new { area = "Authentication" });
		}
	}
}