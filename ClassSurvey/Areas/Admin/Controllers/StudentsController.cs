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
    public class StudentsController : Controller
    {
		//get DB
        private ClassSurveyDbContext db = new ClassSurveyDbContext();

        // GET: Students
        public ActionResult Index()
        {
            return View(db.Students.ToList());
        }

		//find students by searching
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
		//get details of a student
        public ActionResult Details(int? id)
        {
			//not found id
            if (id == null)
            {
				return RedirectToAction("Page404", "Authentication", new { area = "Authentication" });
			}
            Student student = db.Students.Find(id);
			//not found student
            if (student == null)
            {
				return RedirectToAction("Page404", "Authentication", new { area = "Authentication" });
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
				//if username exists in DB before
				ViewBag.DuplicateError = "Username này đã tồn tại. Vui lòng nhập tên khác";
				return View(student);
			}
			//if all validation is accepted
            if (ModelState.IsValid)
            {
				//hash password before adding to DB
				student.Password = HashPassword.ComputeSha256Hash(student.Password);
                db.Students.Add(student);
                db.SaveChanges();
				//get student added ID
				int id = db.Students.Max(x => x.Id);
				//add user
				User user = new User()
				{
					Username = student.Username,
					Password = student.Password,
					Position = "Student",
					StudentId = id
				};
				db.Users.Add(user);
				db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(student);
        }

		//import data from excel
		[HttpPost]
		public ActionResult ImportStudentFromExcel(HttpPostedFileBase fileUpload)
		{
			int count = 0;
			var package = new ExcelPackage(fileUpload.InputStream);
			if (ImportData(out count, package))
			{
				ViewBag.message = "Bạn đã import dữ liệu sinh viên thành công";
			}
			return RedirectToAction("Index","Students", new { area = "Admin"});
		}

		//get data to above function
		public bool ImportData(out int count, ExcelPackage package)
		{
			count = 0;
			var result = false;
			try
			{
				//data start at column 1 and row 2
				int startColumn = 1;
				int startRow = 2;
				ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
				object data = null;
				//get DB
				ClassSurveyDbContext db = new ClassSurveyDbContext();

				do
				{
					data = worksheet.Cells[startRow, startColumn].Value;
					//get Username
					object Username = worksheet.Cells[startRow, startColumn + 1].Value;
					//get password
					object Password = worksheet.Cells[startRow, startColumn + 2].Value;
					//get Fullname
					object Fullname = worksheet.Cells[startRow, startColumn + 3].Value;
					//get Email
					object Email = worksheet.Cells[startRow, startColumn + 4].Value;
					//get ClassByGrade
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

		//check ability to save new student from excel
		public bool SaveStudent(string username, string password, string fullname, string email, string classbygrade, ClassSurveyDbContext db)
		{
			var result = false;
			try
			{
				//save student
				//if students exist before, then not import again
				//just import new student not exists in system
				if (db.Students.Where(x => x.Username.Equals(username)).Count() == 0)
				{
					var student = new Student();
					student.Username = username;
					//hash password before adding
					student.Password = HashPassword.ComputeSha256Hash(password);
					student.StudentCode = username;
					student.StudentName = fullname;
					student.Email = email;
					student.ClassByGrade = classbygrade;
					db.Students.Add(student);
					db.SaveChanges();

					int id = db.Students.Max(x => x.Id);
					//add new user
					User user = new User()
					{
						Username = username,
						Password = HashPassword.ComputeSha256Hash(password),
						Position = "Student",
						StudentId = id
					};
					db.Users.Add(user);
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
				return RedirectToAction("Page404", "Authentication", new { area = "Authentication" });
			}
            Student student = db.Students.Find(id);
            if (student == null)
            {
				return RedirectToAction("Page404", "Authentication", new { area = "Authentication" });
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
				student.Password = HashPassword.ComputeSha256Hash(student.Password);
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
				return RedirectToAction("Page404", "Authentication", new { area = "Authentication" });
			}

			//tim Student theo id
            Student student = db.Students.Find(id);

			//khong tim thay student
            if (student == null)
            {
				return RedirectToAction("Page404", "Authentication", new { area = "Authentication" });
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
				return RedirectToAction("Page404", "Authentication", new { area = "Authentication" });
			}

			User user = db.Users.FirstOrDefault(x => x.StudentId == id);

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
			db.Users.Remove(user);
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
