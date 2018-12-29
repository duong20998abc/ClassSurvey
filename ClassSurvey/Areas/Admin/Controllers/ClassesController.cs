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
			//get list classes
			return View(db.Classes.ToList());
		}

		// GET: Classes/Details/5
		public ActionResult Details(int? id)
		{
			//if id null
			if (id == null)
			{
				return RedirectToAction("Page404", "Authentication", new { area = "Authentication" });
			}
			//get class
			Class @class = db.Classes.Find(id);

			//not found class
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

		//create new class
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

		//  HttpPost method
		// POST: Classes/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			//get class
			Class @class = db.Classes.Find(id);

			//not found class
			if (@class == null)
			{
				return RedirectToAction("Page404", "Authentication", new { area = "Authentication" });
			}
			//remove class
			db.Classes.Remove(@class);

			// when remove class, we have to remove all students in that class as well as their survey results
			//get list students in class
			IEnumerable<StudentClass> listStudentsInClass = db.StudentClasses.Where(x => x.ClassId == id).ToList();
			foreach (var item in listStudentsInClass)
			{
				//remove surveys
				db.Surveys.RemoveRange(item.Surveys);
			}

			//remove students
			db.StudentClasses.RemoveRange(listStudentsInClass);
			db.SaveChanges();
			return RedirectToAction("Index");
		}

		//import classes from file xlsx
		//method HttpPost
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
					//if success
					ViewData["message"] = "Xin chúc mừng. Bạn đã import lớp môn học thành công";
					//count students added
					ViewData["count"] = successNumber.ToString() + "sinh viên đã được thêm vào";
					//dcount students not added
					ViewData["countFail"] = (count - successNumber).ToString() + "sinh viên chưa được thêm";
				}
			}
			catch (Exception)
			{
			}
			return RedirectToAction("Index", "Classes", new { area = "Admin" });
		}

		//get data to above function
		public bool ImportData(out int count, out int successNumber, ExcelPackage excel)
		{
			count = 0;
			successNumber = 0;
			var result = false;
			try
			{
				//add student, student info in the file starts at column 1 and row 12
				int startColumn = 1;
				int startRow = 12;
				ExcelWorksheet worksheet = excel.Workbook.Worksheets[1];
				ClassSurveyDbContext db = new ClassSurveyDbContext();
				object data = null;
				//C7: teacherName
				object teacherName = worksheet.Cells[7, 3].Value;
				//C10: ClassName
				object className = worksheet.Cells[10, 3].Value;
				//C9: ClassCode
				object classCode = worksheet.Cells[9, 3].Value;
				//F9: Number of degrees
				object numberOfDegrees = worksheet.Cells[9, 6].Value;

				//add data
				if (!db.Classes.Any(x => x.ClassCode.ToLower().Equals(classCode.ToString().ToLower())))
				{
					//new class
					Class @class = new Class();
					@class.ClassName = className.ToString();
					@class.ClassCode = classCode.ToString();
					@class.Semester = 1;
					@class.NumberOfDegrees = int.Parse(numberOfDegrees.ToString());
					db.Classes.Add(@class);
					db.SaveChanges();

					//class Id
					int classId = db.Classes.Max(c => c.Id);
					//teacher Id
					int teacherId = db.Teachers.FirstOrDefault(t => t.TeacherName.ToLower()
					.Equals(teacherName.ToString().ToLower())).Id;

					do
					{
						data = worksheet.Cells[startRow, startColumn].Value;
						//get student name
						object studentName = worksheet.Cells[startRow, startColumn + 2].Value;
						//get student code 
						object studentCode = worksheet.Cells[startRow, startColumn + 1].Value;
						//get class by grade
						object classByGrade = worksheet.Cells[startRow, startColumn + 4].Value;
						//get username
						string username = worksheet.Cells[startRow, startColumn + 1].Value.ToString();
						startRow++;
						//if data exists
						if (data != null)
						{
							//get students in that class
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

		//Show result of surveys
		public ActionResult ShowSurveyResult(int? id)
		{
			//not found id
			if (id == null)
			{
				return RedirectToAction("Page404", "Authentication", new { area = "Authentication" });
			}
			//get class
			Class @class = db.Classes.FirstOrDefault(c => c.Id == id);
			//not found class
			if (@class == null)
			{
				return RedirectToAction("Page404", "Authentication", new { area = "Authentication" });
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

			//get standard deviation of survey criteria in 1 class
			List<int> listPointsEachStudent = new List<int>();
			List<double> listSTD = new List<double>();
			foreach (var item in ViewBag.Points)
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
			foreach(var item in ViewBag.PointsAll)
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

			//lay ra danh sach cau hoi khao sat
			ViewBag.SurveyQuestion = db.SurveyQuestions.Select(sc => sc.Content).ToList();

			//lay ra tong so cau hoi trong bai khao sat
			ViewBag.CountQuestion = db.SurveyQuestions.ToList().Count();
			return View(student);
		}

		//get list students in class
		public ActionResult ShowStudentsInClass(int? id)
		{
			if (id == null)
			{
				return RedirectToAction("Page404", "Authentication", new { area = "Authentication" });
			}

			IEnumerable<StudentClass> listStudents = db.StudentClasses.Where(s => s.ClassId == id).ToList();
			//not found
			if (listStudents == null)
			{
				return RedirectToAction("Page404", "Authentication", new { area = "Authentication" });
			}
			return View(listStudents);
		}

		//show student survey result
		public ActionResult ShowSurveyResultByStudent(int? id)
		{
			if (id == null)
			{
				return RedirectToAction("Page404", "Authentication", new { area = "Authentication" });
			}
			//number of students have done survey
			ViewBag.CountStudentsHaveSurvey = db.Surveys.Where(s => s.StudentClassId == id).ToList().Count();
			if (ViewBag.CountStudentsHaveSurvey == 0)
			{
				ViewBag.Message = "Chưa có sinh viên nào làm khảo sát đối với môn học";
				return View();
			}

			//get result of the survey
			ViewBag.Points = db.Surveys.Where(s => s.StudentClassId == id).Select(s => s.Result).ToList();
			if (ViewBag.Points == null)
			{
				return RedirectToAction("Page404", "Authentication", new { area = "Authentication" });
			}
			//get survey criteria
			ViewBag.SurveyQuestion = db.SurveyQuestions.Select(sc => sc.Content).ToList();

			//get number of criteria in the survey
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
