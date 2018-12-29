using ClassSurvey.Authorization;
using ClassSurvey.Domain;
using ClassSurvey.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClassSurvey.Areas.Member.Controllers
{
	[AuthorizeBusiness]
	public class TeacherController : Controller
	{
		private ClassSurveyDbContext db = new ClassSurveyDbContext();
		// GET: Member/Teacher
		public ActionResult Index()
		{
			//get user
			User user = Session["User"] as User;
			//if user exists
			if (user != null)
			{
				//count students in class
				ViewBag.CountStudents = db.Teachers.FirstOrDefault(t => t.Id == user.TeacherId)
				.StudentClasses.Count();
				//count number of classes teacher is responsible for
				ViewBag.CountClasses = db.Teachers.FirstOrDefault(t => t.Id == user.TeacherId)
				.StudentClasses.GroupBy(sc => sc.TeacherId).Count();
				return View();
			}
			return View();
		}

		public ActionResult ShowTeacherInfo()
		{
			//get user
			User user = Session["User"] as User;
			//if user exists
			if (user != null)
			{
				//get teacher
				Teacher teacher = db.Teachers.FirstOrDefault(t => t.Id == user.TeacherId);
				return View(teacher);
			}
			return View();
		}

		[HttpPost]
		public ActionResult ShowTeacherInfo(FormCollection form)
		{
			//get user
			User user = Session["User"] as User;
			//get old password from form 
			string password = form["oldpassword"].ToString();
			//hash old password
			string hashedPassword = HashPassword.ComputeSha256Hash(password);
			//get new password from form
			string newPassword = HashPassword.ComputeSha256Hash(form["newpassword"]);
			if (user != null)
			{
				//get teacher
				Teacher teacher = db.Teachers.FirstOrDefault(s => s.Id == user.TeacherId);
				//old password fail
				if (hashedPassword != user.Password)
				{
					Response.Write("<script>alert('Mật khẩu cũ không đúng. Vui lòng kiểm tra lại')</script>");
					return View(teacher);
				}
				//rewrite new password fail
				else if (form["newpassword"].ToString().Trim() != form["repassword"].ToString().Trim())
				{
					Response.Write("<script>alert('Mật khẩu mới không trùng nhau. Vui lòng kiểm tra lại')</script>");
					return View(teacher);
				}
				//get username
				User u = db.Users.FirstOrDefault(us => us.Username == user.Username);
				//update new password
				u.Password = newPassword;
				teacher.Password = newPassword;
				db.SaveChanges();
				Response.Write("<script>alert('Thay đổi mật khẩu thành công')</script>");
				return View(teacher);
			}
			return RedirectToAction("Index", "Authentication", new { area = "Authentication" });
		}

		public ActionResult ShowListClasses()
		{
			//get user
			User user = Session["User"] as User;

			//get list students in class
			//Su dung IEnumerable, thay cho List (neu dung List phai ToList())
			IEnumerable<StudentClass> studentClasses = db.StudentClasses.Where(sc => sc.TeacherId == user.TeacherId)
				.GroupBy(c => c.ClassId).Select(s => s.FirstOrDefault());
			int teacherId = db.Teachers.FirstOrDefault(t => t.Id == user.TeacherId).Id;
			
			return View(studentClasses);
		}

		//get students in 1 class
		public ActionResult GetStudentsInClass(int? id)
		{
			//if id not exist
			if (id == null)
			{
				return HttpNotFound();
			}
			//get class
			Class @class = db.Classes.FirstOrDefault(c => c.Id == id);
			//if class not exist
			if (@class == null)
			{
				return HttpNotFound();
			}

			//get students in that class
			IEnumerable<StudentClass> listStudentInClass = @class.StudentClasses.ToList();
			return View(listStudentInClass);
		}

		//Show result of Survey with class
		public ActionResult ShowSurveyResult(int? id)
		{
			//id not exist
			if (id == null)
			{
				return HttpNotFound();
			}
			//find class
			Class @class = db.Classes.FirstOrDefault(c => c.Id == id);
			if (@class == null)
			{
				return HttpNotFound();
			}

			//get list id of students have done survey
			List<int> listStudentId = @class.StudentClasses.Select(s => s.Id).ToList();

			//count students have done survey
			ViewBag.CountStudentsHaveSurvey = db.Surveys.Where(s => listStudentId.Any(x => x == s.StudentClassId))
				.ToList().Count();

			//get first student in list students in class
			StudentClass student = db.StudentClasses.First(x => x.ClassId == id);

			//number of students in class
			ViewBag.CountStudentsInClass = db.Classes.FirstOrDefault(s => s.Id == id).StudentClasses
				.ToList().Count();

			//not found student
			if (student == null)
			{
				return RedirectToAction("Page404", "Authentication", new { area = "Authentication" });
			}

			//average survey result according to students doing survey in one class
			//can be understood average in 1 class
			//take data from Surveys table
			ViewBag.Average = db.Surveys.Where(s => listStudentId.Any(x => x == s.StudentClassId))
				.GroupBy(x => x.SurveyQuestionId)
				.Select(x => x.Average(k => k.Result)).ToList();

			//get list survey result to each criterion
			ViewBag.Points = db.Surveys.Where(s => listStudentId.Any(x => x == s.StudentClassId))
				.GroupBy(x => x.SurveyQuestionId)
				.Select(s => s.Select(k => k.Result)).ToList();

			//get standard deviation of survey criteria
			List<int> listPointsEachStudent = new List<int>();
			List<double> listSTD = new List<double>();
			foreach(var item in ViewBag.Points)
			{
				listPointsEachStudent.Clear();
				listPointsEachStudent = item;
				double avg = listPointsEachStudent.Average();
				double sumDif = listPointsEachStudent.Select(val => (val - avg) * (val - avg)).Sum();
				double std = Math.Sqrt(sumDif / (listPointsEachStudent.Count - 1));
				listSTD.Add(std);
			}

			ViewBag.Std = listSTD;

			//average survey result according to students doing survey in all classes
			//can be understood average in all classes
			ViewBag.AverageAll = db.Surveys.GroupBy(s => s.SurveyQuestionId).Select(s => s.Average(k => k.Result)).ToList();

			//get standard deviation of survey criteria in all classes
			ViewBag.PointsAll = db.Surveys.GroupBy(s => s.SurveyQuestionId).Select(s => s.Select(k => k.Result)).ToList();
			List<double> listSTDAll = new List<double>();
			foreach (var item in ViewBag.PointsAll)
			{
				listPointsEachStudent.Clear();
				listPointsEachStudent = item;
				double avg = listPointsEachStudent.Average();
				double sumDif = listPointsEachStudent.Select(val => (val - avg) * (val - avg)).Sum();
				double std = Math.Sqrt(sumDif / (listPointsEachStudent.Count - 1));
				listSTDAll.Add(std);
			}

			//get list standard deviation to show in view
			ViewBag.StdAll = listSTDAll;
			//get survey content
			ViewBag.SurveyQuestion = db.SurveyQuestions.Select(sc => sc.Content).ToList();
			//number of criteria in survey
			ViewBag.CountQuestion = db.SurveyQuestions.ToList().Count();
			return View(student);
		}
	}
}