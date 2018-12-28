using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ClassSurvey.Authorization;
using ClassSurvey.Domain;
using ClassSurvey.Domain.Entities;
using OfficeOpenXml;

namespace ClassSurvey.Areas.Admin.Controllers
{
	[AuthorizeBusiness]
	public class ClassesController : Controller
	{
		private ClassSurveyDbContext db = new ClassSurveyDbContext();

		// GET: Classes
		public ActionResult Index()
		{
			//lay ra danh sach lop hoc
			return View(db.Classes.ToList());
		}

		// GET: Classes/Details/5
		public ActionResult Details(int? id)
		{
			//neu id null
			if (id == null)
			{
				return RedirectToAction("Page404", "Authentication", new { area = "Authentication" });
			}
			//lay ra class co Id = id
			Class @class = db.Classes.Find(id);

			//neu khong tim thay class co id
			if (@class == null)
			{
				return RedirectToAction("Page404", "Authentication", new { area = "Authentication" });
			}
			return View(@class);
		}

		// GET: Classes/Create
		public ActionResult Create()
		{
			ViewBag.TeacherId = new SelectList(db.Teachers, "Id", "TeacherName");
			return View();
		}

		//tao moi 1 class
		// POST: Classes/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "Id,ClassName,ClassCode,Semester,NumberOfDegrees,Status,IsDeleted")] Class @class)
		{
			//ModelState.IsValid cho biet rang khong co loi model nao duoc add vao ModelState
			if (ModelState.IsValid)
			{
				//them moi vao database
				db.Classes.Add(@class);
				db.SaveChanges();
				return RedirectToAction("Index");
			}

			return View(@class);
		}

		// GET: Classes/Delete/5
		//phuong thuc HttpGet
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return RedirectToAction("Page404", "Authentication", new { area = "Authentication" });
			}
			Class @class = db.Classes.Find(id);
			if (@class == null)
			{
				return RedirectToAction("Page404", "Authentication", new { area = "Authentication" });
			}
			return View(@class);
		}

		// Phuong thuc HttpPost
		// POST: Classes/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			//tim lop co Id = id
			Class @class = db.Classes.Find(id);

			//khong tim thay class
			if (@class == null)
			{
				return RedirectToAction("Page404", "Authentication", new { area = "Authentication" });
			}
			//xoa lop
			db.Classes.Remove(@class);

			// khi xoa 1 lop hoc phan thi phai xoa toan bo sinh vien trong lop va khao sat ma cac sinh vien da lam
			//lay ra danh sach sinh vien trong lop
			IEnumerable<StudentClass> listStudentsInClass = db.StudentClasses.Where(x => x.ClassId == id).ToList();
			foreach (var item in listStudentsInClass)
			{
				//xoa survey ma sinh vien da lam
				db.Surveys.RemoveRange(item.Surveys);
			}

			//xoa danh sach sinh vien
			db.StudentClasses.RemoveRange(listStudentsInClass);
			db.SaveChanges();
			return RedirectToAction("Index");
		}

		//them du lieu tu file excel
		[HttpPost]
		public ActionResult ImportClassFromExcel(HttpPostedFileBase file)
		{
			ViewBag.message = "Import không thành công";
			int count = 0;
			int successNumber = 0;
			try
			{
				var excelPackage = new ExcelPackage(file.InputStream);
				if (ImportData(out count, out successNumber, excelPackage))
				{
					//thong bao thanh cong
					ViewData["message"] = "Xin chúc mừng. Bạn đã import lớp môn học thành công";
					//dem so sinh vien duoc them vao
					ViewData["count"] = successNumber.ToString() + "sinh viên đã được thêm vào";
					//dem so sinh vien chua duoc them vao
					ViewData["countFail"] = (count - successNumber).ToString() + "sinh viên chưa được thêm";
				}
			}
			catch (Exception)
			{
			}
			return RedirectToAction("Index", "Classes", new { area = "Admin" });
		}

		//ham bool them data, phuc vu cho ham import o tren
		public bool ImportData(out int count, out int successNumber, ExcelPackage excel)
		{
			count = 0;
			successNumber = 0;
			var result = false;
			try
			{
				//qua trinh them danh sach sinh vien, bat dau tu cot 1 va dong 12
				int startColumn = 1;
				int startRow = 12;
				ExcelWorksheet worksheet = excel.Workbook.Worksheets[1];
				ClassSurveyDbContext db = new ClassSurveyDbContext();
				object data = null;
				//ten giao vien nam o C7
				object teacherName = worksheet.Cells[7, 3].Value;
				//ten lop nam o C10
				object className = worksheet.Cells[10, 3].Value;
				//ma lop nam o C9
				object classCode = worksheet.Cells[9, 3].Value;
				//so luong tin chi nam o F9
				object numberOfDegrees = worksheet.Cells[9, 6].Value;

				//them du lieu vao DB
				if (!db.Classes.Any(x => x.ClassCode.ToLower().Equals(classCode.ToString().ToLower())))
				{
					Class @class = new Class();
					@class.ClassName = className.ToString();
					@class.ClassCode = classCode.ToString();
					@class.Semester = 1;
					@class.NumberOfDegrees = int.Parse(numberOfDegrees.ToString());
					db.Classes.Add(@class);
					db.SaveChanges();

					//Ham max la de lay ra id gan nhat, nghia la class id vua tao o tren
					int classId = db.Classes.Max(c => c.Id);
					//lay ra Id cua giao vien ung voi lop hoc phan
					int teacherId = db.Teachers.FirstOrDefault(t => t.TeacherName.ToLower()
					.Equals(teacherName.ToString().ToLower())).Id;

					do
					{
						data = worksheet.Cells[startRow, startColumn].Value;
						//ten hoc sinh
						object studentName = worksheet.Cells[startRow, startColumn + 2].Value;
						//ma hoc sinh
						object studentCode = worksheet.Cells[startRow, startColumn + 1].Value;
						//lop theo khoa
						object classByGrade = worksheet.Cells[startRow, startColumn + 4].Value;
						string username = worksheet.Cells[startRow, startColumn + 1].Value.ToString();
						startRow++;
						if (data != null)
						{
							count++;
							Student student = db.Students.FirstOrDefault(s => s.Username.Trim()
							.Equals(username.Trim()));
							if(student == null)
							{
								continue;
							}
							if (student.StudentCode == null)
							{
								student.StudentCode = username;
							}

							StudentClass studentClass = new StudentClass();
							studentClass.StudentId = student.Id;
							studentClass.TeacherId = teacherId;
							studentClass.ClassId = classId;
							db.StudentClasses.Add(studentClass);
							db.SaveChanges();
							successNumber++;
							result = true;
						}

					} while (data != null);
				}
			}
			catch (Exception)
			{

			}

			return result;
		}

		//In ra ket qua cua cuoc khao sat
		public ActionResult ShowSurveyResult(int? id)
		{
			if (id == null)
			{
				return RedirectToAction("Page404", "Authentication", new { area = "Authentication" });
			}
			//find ra class co Id = id
			Class @class = db.Classes.FirstOrDefault(c => c.Id == id);
			if (@class == null)
			{
				return RedirectToAction("Page404", "Authentication", new { area = "Authentication" });
			}

			//lay ra danh sach id cua sinh vien da danh gia mon hoc (co survey result)
			List<int> listStudentId = @class.StudentClasses.Select(s => s.Id).ToList();

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
				return RedirectToAction("Page404", "Authentication", new { area = "Authentication" });
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

		//lay ra danh sach sinh vien trong 1 lop hoc phan
		public ActionResult ShowStudentsInClass(int? id)
		{
			if (id == null)
			{
				return RedirectToAction("Page404", "Authentication", new { area = "Authentication" });
			}

			//lay ra danh sach hoc sinh trong lop hoc phan co ID = id
			IEnumerable<StudentClass> listStudents = db.StudentClasses.Where(s => s.ClassId == id).ToList();
			//khong tim thay
			if (listStudents == null)
			{
				return RedirectToAction("Page404", "Authentication", new { area = "Authentication" });
			}
			return View(listStudents);
		}

		//Hien thi ket qua survey ung voi moi 1 sinh vien
		public ActionResult ShowSurveyResultByStudent(int? id)
		{
			if (id == null)
			{
				return RedirectToAction("Page404", "Authentication", new { area = "Authentication" });
			}
			//dem so sinh vien da lam khao sat
			ViewBag.CountStudentsHaveSurvey = db.Surveys.Where(s => s.StudentClassId == id).ToList().Count();
			if (ViewBag.CountStudentsHaveSurvey == 0)
			{
				ViewBag.Message = "Chưa có sinh viên nào làm khảo sát đối với môn học";
				return View();
			}

			//lay ra diem ma sinh vien da danh gia
			ViewBag.Points = db.Surveys.Where(s => s.StudentClassId == id).Select(s => s.Result).ToList();
			if (ViewBag.Points == null)
			{
				return RedirectToAction("Page404", "Authentication", new { area = "Authentication" });
			}
			//lay ra danh sach cau hoi khao sat
			ViewBag.SurveyQuestion = db.SurveyQuestions.Select(sc => sc.Content).ToList();

			//lay ra tong so cau hoi trong bai khao sat
			ViewBag.CountQuestion = db.SurveyQuestions.ToList().Count();
			return View();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
