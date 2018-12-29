using ClassSurvey.Authorization;
using ClassSurvey.Domain;
using ClassSurvey.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace ClassSurvey.Areas.Member.Controllers
{
	[AuthorizeBusiness]
    public class StudentController : Controller
    {
		private ClassSurveyDbContext db = new ClassSurveyDbContext();
		// GET: Member/Student
		public ActionResult Index()
		{
			//get user from session
			User user = Session["User"] as User;

			//check user exists
			if (user != null)
			{
				//get student has Id equals to user studentId
				Student student = db.Students.FirstOrDefault(s => s.Id == user.StudentId);
				//number of classes
				ViewBag.CountClass = student.StudentClasses.Count();
				//get listStudentId
				List<int?> listStudentId = student.StudentClasses.Select(s => s.StudentId).ToList();
				//get number of students have survey
				ViewBag.CountSurvey = db.Surveys.Where(s => listStudentId.Any(x => x == s.StudentClassId))
					.GroupBy(x => x.StudentClassId).Count();
				return View();
			}
			return RedirectToAction("Home", "Authentication", new { area = "Authentication" });
		}

		//get student info
		public ActionResult ShowStudentInfo()
		{
			//get user from session
			User user = Session["User"] as User;
			if(user != null)
			{
				//get student when id == user's studentID
				Student student = db.Students.FirstOrDefault(s => s.Id == user.StudentId);
				return View(student);
			}
			return RedirectToAction("Home", "Authentication", new {area = "Authentication" });
		}

		//update password in form
		[HttpPost]
		public ActionResult ShowStudentInfo(FormCollection form)
		{
			//get user from session
			User user = Session["User"] as User;
			//input old password
			string password = form["oldpassword"].ToString();
			//hash old password
			string hashedPassword = HashPassword.ComputeSha256Hash(password);
			//input new password
			string newPassword = HashPassword.ComputeSha256Hash(form["newpassword"]);
			//if user exists
			if(user != null)
			{
				//get student
				Student student = db.Students.FirstOrDefault(s => s.Id == user.StudentId);
				//type old password fail
				if(hashedPassword != user.Password)
				{
					Response.Write("<script>alert('Mật khẩu cũ không đúng. Vui lòng kiểm tra lại')</script>");
					return View(student);
				}
				//rewrite new password fail
				else if(form["newpassword"].ToString().Trim() != form["repassword"].ToString().Trim())
				{
					Response.Write("<script>alert('Mật khẩu mới không trùng nhau. Vui lòng kiểm tra lại')</script>");
					return View(student);
				}
				//get user
				User u = db.Users.FirstOrDefault(us => us.Username == user.Username);
				u.Password = newPassword;
				student.Password = newPassword;
				db.SaveChanges();
				Response.Write("<script>alert('Thay đổi mật khẩu thành công')</script>");
				return View(student);
			}
			return RedirectToAction("Index", "Authentication", new { area = "Authentication" });
		}

		//get classes student participating in
		public ActionResult ShowListClass()
		{
			//get user
			User user = Session["User"] as User;
			if(user != null)
			{
				//get classes
				List<StudentClass> listClasses = db.Students.FirstOrDefault(s => s.Id == user.StudentId)
					.StudentClasses.ToList();
				return View(listClasses);
			}
			return RedirectToAction("Home", "Authentication", new { area = "Authentication" });
		}

		//Do Survey
		public ActionResult DoSurvey (int? id, int? studentId)
		{
			//if id or studentID not exists
			if(id == null || studentId == null)
			{
				//return 404
				return RedirectToAction("Page404", "Authentication", new { area = "Authentication" });
			}

			//get student with id and studentid
			StudentClass studentClass = db.StudentClasses.FirstOrDefault(x => x.StudentId == studentId && x.ClassId == id);
			//not found student
			if(studentClass == null)
			{
				return RedirectToAction("Page404", "Authentication", new { area = "Authentication" });
			}

			//check if student has survey or not
			if(db.Surveys.Any(s => s.StudentClassId == studentClass.Id))
			{
				ViewBag.Message = "Môn học này đã được đánh giá. Cảm ơn bạn đã ghé thăm";
				return View(studentClass);
			}

			//number of criteria in survey table
			ViewBag.CountQuestion = db.SurveyQuestions.ToList().Count();

			//list criteria in the survey
			ViewBag.SurveyQuestions = db.SurveyQuestions.Select(sq => sq.Content).ToList();
			return View(studentClass);
		}

		//post survey form
		[HttpPost]
		public ActionResult DoSurvey(FormCollection form)
		{
			//number of criteria in survey table
			ViewBag.CountQuestion = db.SurveyQuestions.ToList().Count();
			//list criteria in the survey
			ViewBag.SurveyQuestions = db.SurveyQuestions.Select(sq => sq.Content).ToList();
			//get class id from form has name = classId
			int classId = int.Parse(form["classId"]);
			//get studentId from form
			int studentId = int.Parse(form["studentdetailId"]);
			//get student
			StudentClass studentClass = db.StudentClasses.FirstOrDefault(sc => sc.ClassId == classId);
			//check if all criteria have result
			if(form.Count < db.SurveyQuestions.ToList().Count() + 2)
			{
				ViewBag.Message = "Bạn cần phải hoàn thiện việc đánh giá tất cả các tiêu chí trước khi submit";
				return View(studentClass);
			}
			int i = 0;
			foreach(var item in db.SurveyQuestions)
			{
				Survey survey = new Survey();
				survey.StudentClassId = studentId;
				survey.SurveyQuestionId = item.Id;
				survey.Result = int.Parse(form[i++]);
				db.Surveys.Add(survey);
			}
			StudentClass stu = db.StudentClasses.FirstOrDefault(sc => sc.Id == studentId);
			db.SaveChanges();
			Response.Write("<script>alert('Đánh giá môn học thành công')</script>");
			return RedirectToAction("ShowListClass");
		}
    }
}