using ClassSurvey.Domain;
using ClassSurvey.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClassSurvey.Areas.Authentication.Controllers
{
    public class AuthenticationController : Controller
    {
		private ClassSurveyDbContext db = new ClassSurveyDbContext();
        // GET: Authentication/Authentication
        public ActionResult Index()
		{
			//lay ra user thong qua session
			User user = Session["User"] as User;
			//neu ton tai user
			if(user != null)
			{
				//neu user la teacher
				if(user.Position.Equals("Teacher"))
				{
					//lay ra teacher theo id
					Teacher teacher = db.Teachers.FirstOrDefault(t => t.Id == user.TeacherId);
					//luu ten teacher vao session
					Session["Username"] = teacher.TeacherName.ToUpper();
					//redirect den trang cua giao vien
					//actionName = Index, controllerName = Teacher, routeValue = Area Member
					return RedirectToAction("Index", "Teacher", new { area = "Member" });
				}
				else if (user.Position.Equals("Student"))
				{
					//lay ra student theo id
					Student student = db.Students.FirstOrDefault(s => s.Id == user.StudentId);
					//luu ten teacher vao session
					Session["Username"] = student.StudentName.ToUpper();
					//redirect den trang cua sinh vien
					//actionName = Index, controllerName = Student, routeValue = Area Member
					return RedirectToAction("Index", "Student", new { area = "Member" });
				}else
				{
					//luu ten admin vao session
					Session["Username"] = "Admin";
					//redirect den trang cua admin
					//actionName = Index, controllerName = Home, routeValue = Area Admin
					return RedirectToAction("Index", "Home", new { area = "Admin" });
				}
			}
			return View("Index",new {area = "Authentication" });
		}

		[HttpPost]
		public ActionResult LoginFromForm(FormCollection form)
		{
			//lay ra username nhap vao tu form
			string Username = form["login-user"].ToString().Trim();
			//lay ra password nhap vao tu form
			string Password = form["login-password"].ToString().Trim();
			//tim user co username va password nhu vua nhap
			User user = db.Users.FirstOrDefault(u => u.Username == Username && u.Password == Password);
			if(user != null)
			{
				Session["User"] = user;
				if(user.Position.Equals("Teacher"))
				{
					//lay ra teacher theo id
					Teacher teacher = db.Teachers.FirstOrDefault(t => t.Id == user.TeacherId);
					//luu ten teacher vao session
					Session["Username"] = teacher.TeacherName.ToUpper();
					//redirect den trang cua giao vien
					//actionName = Index, controllerName = Teacher, routeValue = Area Member
					return RedirectToAction("Index", "Teacher", new { area = "Member" });
				}
				else if (user.Position.Equals("Student"))
				{
					//lay ra student theo id
					Student student = db.Students.FirstOrDefault(s => s.Id == user.StudentId);
					//luu ten teacher vao session
					Session["Username"] = student.StudentName.ToUpper();
					//redirect den trang cua sinh vien
					//actionName = Index, controllerName = Student, routeValue = Area Member
					return RedirectToAction("Index", "Student", new { area = "Member" });
				}
				else
				{
					//luu ten admin vao session
					Session["Username"] = "Admin";
					//redirect den trang cua admin
					//actionName = Index, controllerName = Home, routeValue = Area Admin
					return RedirectToAction("Index", "Home", new { area = "Admin" });
				}
			}
			return View("Index", new { area = "Authentication" });
		}

		public ActionResult Logout()
		{
			Session.Remove("User");
			Session.Remove("Username");
			return RedirectToAction("Index", new {area = "Authentication" });
		}

		public ActionResult NotAuthorized()
		{
			return View();
		}

		public ActionResult Page404()
		{
			return View();
		} 


    }
}