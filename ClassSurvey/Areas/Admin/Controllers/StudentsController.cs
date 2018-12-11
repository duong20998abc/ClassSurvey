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
    public class StudentsController : Controller
    {
        private ClassSurveyDbContext db = new ClassSurveyDbContext();

        // GET: Students
        public ActionResult Index()
        {
            return View(db.Students.ToList());
        }

		//phuong thuc tim kiem theo ten hoc sinh
		[HttpPost]
		public ActionResult Index(FormCollection form)
		{
			string keyword = form["keyword"].ToString();
			ViewBag.Keyword = keyword;
			List<Student> listStudentsWithKeywords = db.Students.Where(x => x.StudentName.Contains(keyword)).ToList();
			if(listStudentsWithKeywords.Count == 0)
			{
				ViewBag.ErrorMessage = "Không tìm thấy sinh viên nào. Vui lòng thử lại";
				return View(db.Students.ToList());
			}
			else
			{
				ViewBag.SuccessMessage = "Danh sách sinh viên với từ khóa: " + keyword;
				ViewBag.Count = listStudentsWithKeywords.Count();
				return View(listStudentsWithKeywords.ToList());
			}
		}

        // GET: Students/Details/5
		//lay ra chi tiet sinh vien
        public ActionResult Details(int? id)
        {
			//khong ton tai id => tra ve Bad request
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
			//Khong tim thay sinh vien co Id => HttpNotFound
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: Students/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// Tao moi 1 sinh vien, trong do ValidateAntiForgeryToken de ngan bad request den tu user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,StudentCode,StudentName,Email,ClassByGrade,Username,Password,Status,IsDeleted")] Student student)
        {
			if(db.Students.Any(x => x.Username == student.Username))
			{
				ViewBag.DuplicateError = "Username này đã tồn tại. Vui lòng nhập tên khác";
				return View(student);
			}
            if (ModelState.IsValid)
            {
                db.Students.Add(student);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(student);
        }

		//import data from excel
		[HttpPost]
		public ActionResult ImportStudentFromExcel(HttpPostedFileBase fileUpload)
		{
			ViewBag.message = "Import không thành công";
			int count = 0;
			var package = new ExcelPackage(fileUpload.InputStream);
			if (ImportData(out count, package))
			{
				ViewBag.message = "Bạn đã import dữ liệu sinh viên thành công";
			}

			ViewBag.countStudent = count;
			return View();
		}

		public bool ImportData(out int count, ExcelPackage package)
		{
			count = 0;
			var result = false;
			try
			{
				int startColumn = 1;
				int startRow = 2;
				ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
				object data = null;
				ClassSurveyDbContext db = new ClassSurveyDbContext();

				do
				{
					data = worksheet.Cells[startRow, startColumn].Value;
					object Username = worksheet.Cells[startRow, startColumn + 1].Value;
					object Password = worksheet.Cells[startRow, startColumn + 2].Value;
					object Fullname = worksheet.Cells[startRow, startColumn + 3].Value;
					object Email = worksheet.Cells[startRow, startColumn + 4].Value;
					object ClassByGrade = worksheet.Cells[startRow, startColumn + 5].Value;

					if (data != null)
					{
						var isImported = SaveStudent(Username.ToString(), Password.ToString(), Fullname.ToString()
							, Email.ToString(), ClassByGrade.ToString(), db);
						if (isImported)
						{
							count++;
							result = true;
						}
					}

					startRow++;
				} while (data != null);
			}
			catch (Exception)
			{

				throw;
			}
			return result;
		}

		public bool SaveStudent(string username, string password, string fullname, string email, string classbygrade, ClassSurveyDbContext db)
		{
			var result = false;
			try
			{
				if (db.Students.Where(x => x.Username.Equals(username)).Count() == 0)
				{
					var student = new Student();
					student.Username = username;
					student.Password = password;
					student.StudentCode = username;
					student.StudentName = fullname;
					student.Email = email;
					student.ClassByGrade = classbygrade;
					db.Students.Add(student);
					db.SaveChanges();
					result = true;
				}
			}
			catch (Exception)
			{

				throw;
			}
			return result;
		}

		// GET: Students/Edit/5
		public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,StudentCode,StudentName,Email,ClassByGrade,Username,Password,Status,IsDeleted")] Student student)
        {
            if (ModelState.IsValid)
            {
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(student);
        }

        // GET: Students/Delete/5
        public ActionResult Delete(int? id)
        {
			//khong tim thay id
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

			//tim Student theo id
            Student student = db.Students.Find(id);

			//khong tim thay student
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
			//tim Student theo id
            Student student = db.Students.Find(id);

			//neu khong tim thay student
			if(student == null)
			{
				return HttpNotFound();
			}

			//lay ra danh sach hoc sinh hoc trong lop
			IEnumerable<StudentClass> listStudentsInClass = db.StudentClasses.Where(s => s.StudentId == id);
			foreach(var item in listStudentsInClass)
			{
				//lay ra danh sach survey sinh vien da thuc hien
				IEnumerable<Survey> listSurveysByStudents = db.Surveys.Where(s => s.StudentClassId == item.Id);
				try
				{
					//xoa survey sinh vien da thuc hien
					db.Surveys.RemoveRange(listSurveysByStudents);
				}
				catch (Exception)
				{

					throw;
				}
			}
			//xoa danh sach hoc sinh trong lop
			db.StudentClasses.RemoveRange(listStudentsInClass);

			//xoa hoc sinh
            db.Students.Remove(student);
            db.SaveChanges();
            return RedirectToAction("Index");
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
