using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ClassSurvey.Domain;
using ClassSurvey.Domain.Entities;
using OfficeOpenXml;

namespace ClassSurvey.Controllers
{
    public class ClassesController : Controller
    {
        private ClassSurveyDbContext db = new ClassSurveyDbContext();

        // GET: Classes
        public ActionResult Index()
        {
            return View(db.Classes.ToList());
        }

        // GET: Classes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Class @class = db.Classes.Find(id);
            if (@class == null)
            {
                return HttpNotFound();
            }
            return View(@class);
        }

        // GET: Classes/Create
        public ActionResult Create()
        {
            ViewBag.TeacherId = new SelectList(db.Teachers, "Id", "TeacherName");
            return View();
        }

        // POST: Classes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ClassName,ClassCode,Semester,NumberOfDegrees,Status,IsDeleted")] Class @class)
        {
            if (ModelState.IsValid)
            {
                db.Classes.Add(@class);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(@class);
        }

        // GET: Classes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Class @class = db.Classes.Find(id);
            if (@class == null)
            {
                return HttpNotFound();
            }

            return View(@class);
        }

        // POST: Classes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ClassName,ClassCode,Semester,NumberOfDegrees,Status,IsDeleted")] Class @class)
        {
            if (ModelState.IsValid)
            {
                db.Entry(@class).State = EntityState.Modified;
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
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Class @class = db.Classes.Find(id);
            if (@class == null)
            {
                return HttpNotFound();
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
			if(@class == null)
			{
				return HttpNotFound();
			}
            db.Classes.Remove(@class);
			IEnumerable<StudentClass> listStudentsInClass = db.StudentClasses.Where(x => x.ClassId == id).ToList();
			foreach(var item in listStudentsInClass)
			{
				db.Surveys.RemoveRange(item.Surveys);
			}

			db.StudentClasses.RemoveRange(listStudentsInClass);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

		[HttpPost]
		public ActionResult ImportClassFromExcel (HttpPostedFileBase file)
		{
			int count = 0;
			int successNumber = 0;
			try
			{
				var excelPackage = new ExcelPackage(file.InputStream);
				if(ImportData(out count, out successNumber, excelPackage))
				{
					ViewBag.Message = "Xin chúc mừng. Bạn đã import lớp môn học thành công";
					ViewBag.Count = successNumber.ToString() + "sinh viên đã được thêm vào";
					ViewBag.CountFail = (count - successNumber).ToString() + "sinh viên chưa được thêm";
				}
			}
			catch (Exception)
			{
				throw;
			}
			return View();
		}

		public bool ImportData (out int count, out int successNumber, ExcelPackage excel )
		{
			count = 0;
			successNumber = 0;
			var result = false;
			try
			{	
				ExcelWorksheet worksheet = excel.Workbook.Worksheets[1];
				ClassSurveyDbContext db = new ClassSurveyDbContext();
				object teacherName = worksheet.Cells[7, 3].Value;
				object className = worksheet.Cells[10, 3].Value;
				object classCode = worksheet.Cells[9, 3].Value;
				object numberOfDegrees = worksheet.Cells[9, 6].Value;

				if(!db.Classes.Any(x => x.ClassCode.ToLower().Equals(classCode.ToString().ToLower())))
				{
					Class @class = new Class();
					@class.ClassName = className.ToString();
					@class.ClassCode = classCode.ToString();
					@class.Semester = 1;
					@class.NumberOfDegrees = int.Parse(numberOfDegrees.ToString());

					//Ham max la de lay ra id gan nhat, nghia la class id vua tao o tren
					int classId = db.Classes.Max(c => c.Id);
					//lay ra Id cua giao vien ung voi lop hoc phan
					int teacherId = db.Teachers.FirstOrDefault(t => t.TeacherName.ToLower()
					.Equals(teacherName.ToString().ToLower())).Id;

					int startColumn = 1;
					int startRow = 12;
					object data = null;
					do
					{
						data = worksheet.Cells[startRow, startColumn].Value;
						object studentName = worksheet.Cells[startRow, startColumn + 2];
						object studentCode = worksheet.Cells[startRow, startColumn + 1];
						object classByGrade = worksheet.Cells[startRow, startColumn + 4];

						if(data != null)
						{
							count++;
							Student student = db.Students.FirstOrDefault(s => s.StudentCode.Trim()
							.Equals(studentCode.ToString().Trim()));
							if(student.Username == null)
							{
								student.Username = student.StudentCode;
							}

							StudentClass studentClass = new StudentClass();
							studentClass.StudentId = student.Id;
							studentClass.TeacherId = teacherId;
							studentClass.ClassId = classId;
							db.StudentClasses.Add(studentClass);
							db.SaveChanges();
							successNumber++;
							return true;
						}

					} while (data != null);
				}
			}
			catch (Exception)
			{

				throw;
			}

			return result;
		}

		public ActionResult ShowSurveyResult(int? id)
		{
			if(id == null)
			{
				return HttpNotFound();
			}
			Class @class = db.Classes.FirstOrDefault(c => c.Id == id);
			if(@class == null)
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
			if(student == null)
			{
				return HttpNotFound();
			}

			/*tinh diem danh gia trung binh qua cac cuoc khao sat cua sinh vien
			doi voi cac cau hoi trong bang khao sat*/
			
			ViewBag.Average = db.Surveys.Where(s => listStudentId.Any(x => x == s.StudentClassId))
				.GroupBy(x => x.SurveyQuestionId)
				.Select(x => x.Average(k => k.Result)).ToList();

			return View(student);
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
