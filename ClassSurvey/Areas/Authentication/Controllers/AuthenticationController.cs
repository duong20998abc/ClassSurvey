using ClassSurvey.Authorization;
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

        public ActionResult Home()
		{
			//get user by session
			User user = Session["User"] as User;
			//if exist user
			if(user != null)
			{
				//teacher
				if(user.Position.Equals("Teacher"))
				{
					//get teacher by id
					//if id of teacher equals teacherId of user
					Teacher teacher = db.Teachers.FirstOrDefault(t => t.Id == user.TeacherId);
					//add teacher to session
					Session["Username"] = teacher.TeacherName.ToUpper();
					//redirect to teacher page
					//actionName = Index, controllerName = Teacher, routeValue = Area Member
					return RedirectToAction("Index", "Teacher", new { area = "Member" });
				}
				//student
				else if (user.Position.Equals("Student"))
				{
					//get student by id
					Student student = db.Students.FirstOrDefault(s => s.Id == user.StudentId);
					//add student to session
					Session["Username"] = student.StudentName.ToUpper();
					//redirect to student page
					//actionName = Index, controllerName = Student, routeValue = Area Member
					return RedirectToAction("ShowListClass", "Student", new { area = "Member" });
				}else
				{
					//add admin to session
					Session["Username"] = "Admin";
					//redirect to admin index page
					//actionName = Index, controllerName = Home, routeValue = Area Admin
					return RedirectToAction("Index", "Home", new { area = "Admin" });
				}
			}
			return View("~/Areas/Authentication/Views/Authentication/Home.cshtml");
		}

		[HttpPost]
		public ActionResult Home(FormCollection form)
		{
			//lay ra username nhap vao tu form
			string Username = form["login-user"].ToString().Trim();
			//lay ra password nhap vao tu form
			string Password = form["login-password"].ToString().Trim();
			//tim user co username va password nhu vua nhap
			string hashedPassword = HashPassword.ComputeSha256Hash(Password);
			User user = db.Users.FirstOrDefault(u => u.Username.Equals(Username) && u.Password.Equals(hashedPassword));
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
					return RedirectToAction("ShowListClass", "Student", new { area = "Member" });
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
			return View();
		}

		//Logout
		//When logout, session about user and username is removed and return back to the default page
		public ActionResult Logout()
		{
			Session.Remove("User");
			Session.Remove("Username");
			return RedirectToAction("Home","Authentication", new {area = "Authentication" });
		}

		//page not authorized, return status 403 page
		public ActionResult NotAuthorized()
		{
			return View();
		}

		//page not found, return status 404 page
		public ActionResult Page404()
		{
			return View();
		} 
    }
}