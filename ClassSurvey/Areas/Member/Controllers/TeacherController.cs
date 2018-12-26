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
			//lay ra user tu session
			User user = Session["User"] as User;
			//neu ton tai user
			if (user != null)
			{
				//Dem so luong sinh vien giao vien dang day
				ViewBag.CountStudents = db.Teachers.FirstOrDefault(t => t.Id == user.TeacherId)
				.StudentClasses.Count();
				//Dem so lop giao vien day
				ViewBag.CountClasses = db.Teachers.FirstOrDefault(t => t.Id == user.TeacherId)
				.StudentClasses.GroupBy(sc => sc.TeacherId).Count();
				return View();
			}
			return View();
		}

		public ActionResult ShowTeacherInfo()
		{
			//lay ra user tu session
			User user = Session["User"] as User;
			//neu ton tai user
			if (user != null)
			{
				Teacher teacher = db.Teachers.FirstOrDefault(t => t.Id == user.TeacherId);
				return View(teacher);
			}
			return View();
		}

		public ActionResult ShowListClasses()
		{
			//lay ra user tu session
			User user = Session["User"] as User;

			//lay ra danh sach sinh vien trong lop giao vien day
			//Su dung IEnumerable, thay cho List (neu dung List phai ToList())
			IEnumerable<StudentClass> studentClasses = db.StudentClasses.Where(sc => sc.TeacherId == user.TeacherId)
				.GroupBy(c => c.ClassId).Select(s => s.FirstOrDefault());
			return View(studentClasses);
		}

		//lay ra danh sach sinh vien trong 1 lop hoc phan
		public ActionResult GetStudentsInClass(int? id)
		{
			//neu ko ton tai id
			if (id == null)
			{
				return HttpNotFound();
			}
			//lay ra class co Id = id
			Class @class = db.Classes.FirstOrDefault(c => c.Id == id);
			//neu ko ton tai class
			if (@class == null)
			{
				return HttpNotFound();
			}

			//lay ra danh sach hoc sinh tuong ung voi lop hoc phan (va ca giao vien day lop do)
			IEnumerable<StudentClass> listStudentInClass = @class.StudentClasses.ToList();
			return View(listStudentInClass);
		}

		//In ra ket qua cua cuoc khao sat
		//giong ham trong ClassesController (admin)
		public ActionResult ShowSurveyResult(int? id)
		{
			if (id == null)
			{
				return HttpNotFound();
			}
			//find ra class co Id = id
			Class @class = db.Classes.FirstOrDefault(c => c.Id == id);
			if (@class == null)
			{
				return HttpNotFound();
			}

			//lay ra danh sach id cua sinh vien da danh gia mon hoc (co survey result)
			List<int> listStudentId = db.StudentClasses.Select(s => s.Id).ToList();

			//dem so luong sinh vien da danh gia
			ViewBag.CountStudentsHaveSurvey = db.Surveys.Where(s => listStudentId.Any(x => x == s.StudentClassId))
				.ToList().Count();

			//lay ra sinh vien dau tien trong danh sach sinh vien trong lop
			StudentClass student = db.StudentClasses.First(x => x.ClassId == id);

			//lay ra tong so sinh vien trong lop
			ViewBag.CountStudentsInClass = db.Classes.FirstOrDefault(s => s.Id == id).StudentClasses
				.ToList().Count();

			//khong co sinh vien duoc tim thay
			if (student == null)
			{
				return HttpNotFound();
			}

			/*tinh diem danh gia trung binh qua cac cuoc khao sat cua sinh vien
			doi voi cac cau hoi trong bang khao sat*/

			ViewBag.Average = db.Surveys.Where(s => listStudentId.Any(x => x == s.StudentClassId))
				.GroupBy(x => x.SurveyQuestionId)
				.Select(x => x.Average(k => k.Result)).ToList();

			//lay ra danh sach cau hoi khao sat
			ViewBag.SurveyQuestion = db.SurveyQuestions.Select(sc => sc.Content).ToList();

			//lay ra tong so cau hoi trong bai khao sat
			ViewBag.CountQuestion = db.SurveyQuestions.ToList().Count();
			return View(student);
		}
	}
}