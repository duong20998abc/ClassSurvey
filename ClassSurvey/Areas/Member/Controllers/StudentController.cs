using ClassSurvey.Domain;
using ClassSurvey.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClassSurvey.Areas.Member.Controllers
{
    public class StudentController : Controller
    {
		private ClassSurveyDbContext db = new ClassSurveyDbContext();
		// GET: Member/Student
		public ActionResult Index()
		{
			//lay ra user tu session
			User user = Session["User"] as User;

			//check user ton tai
			if (user != null)
			{
				//lay ra student co Id trung voi studentId cua user
				Student student = db.Students.FirstOrDefault(s => s.Id == user.StudentId);
				//lay tong so mon hoc
				ViewBag.CountClass = student.StudentClasses.Count();
				//lay ra list id cua student thong qua bang StudentClass (select ra studentid)
				List<int> listStudentId = student.StudentClasses.Select(s => s.StudentId).ToList();
				//lay ra tong so mon hoc sinh vien da lam khao sat
				ViewBag.CountSurvey = db.Surveys.Where(s => listStudentId.Any(x => x == s.StudentClassId))
					.GroupBy(x => x.StudentClassId).Count();
				return View();
			}
			return View();
		}

		//lay ra thong tin cua sinh vien
		public ActionResult ShowStudentInfo()
		{
			//lay ra user tu session
			User user = Session["User"] as User;
			if(user != null)
			{
				//lay ra sinh vien co Id = studentId cua user
				Student student = db.Students.FirstOrDefault(s => s.Id == user.StudentId);
				return View(student);
			}
			return RedirectToAction("Login", "Home", new {area = "SignIn" });
		}

		//lay ra danh sach lop ma sinh vien tham gia
		public ActionResult ShowListClass()
		{
			//lay ra session cua user
			User user = Session["User"] as User;
			if(user != null)
			{
				//lay danh sach lop sinh vien tham gia thong qua bang studentclass
				List<StudentClass> listClasses = db.Students.FirstOrDefault(s => s.Id == user.StudentId)
					.StudentClasses.ToList();
				return View(listClasses);
			}
			return RedirectToAction("Login", "Home", new { area = "SignIn" });
		}

		//Thuc hien khao sat mon hoc
		public ActionResult DoSurvey (int? id, int? studentId)
		{
			//neu ko ton tai id va studentId
			if(id == null || studentId == null)
			{
				//tra ve trang bao loi
				return HttpNotFound();
			}

			//lay ra sinh vien co studentId va classId, chi tiet ve mon hoc va sinh vien do
			StudentClass studentClass = db.StudentClasses.FirstOrDefault(x => x.StudentId == studentId && x.ClassId == id);
			if(studentClass == null)
			{
				return HttpNotFound();
			}

			//kiem tra xem sinh vien da lam khao sat chua
			if(db.Surveys.Any(s => s.StudentClassId == studentClass.Id))
			{
				ViewBag.Message = "Môn học này đã được đánh giá. Cảm ơn bạn đã ghé thăm";
				return View(studentClass);
			}

			//tong so cau hoi trong bai khao sat
			ViewBag.CountQuestion = db.SurveyQuestions.ToList().Count();

			//danh sach cau hoi trong bai khao sat
			ViewBag.SurveyQuestions = db.SurveyQuestions.Select(sq => sq.Content).ToList();
			return View(studentClass);
		}

		//ham xu ly khi sinh vien post dap an cho bai khao sat
		[HttpPost]
		public ActionResult DoSurvey(FormCollection form, int classId, int studentId)
		{
			//tong so cau hoi trong bai khao sat
			ViewBag.CountQuestion = db.SurveyQuestions.ToList().Count();
			//danh sach cau hoi trong bai khao sat
			ViewBag.SurveyQuestions = db.SurveyQuestions.Select(sq => sq.Content).ToList();
			//lay ra id lop tu form co name = classId
			classId = int.Parse(form["classId"]);
			//lay ra id cua student tu form co name = studentId
			studentId = int.Parse(form["studentId"]);
			//lay ra sinh vien trong lop co classId = classId lay ra tu form
			StudentClass studentClass = db.StudentClasses.FirstOrDefault(sc => sc.ClassId == classId);
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
			return RedirectToAction("ShowListClass");
		}
    }
}