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
    public class TeachersController : Controller
    {
        private ClassSurveyDbContext db = new ClassSurveyDbContext();

        // GET: Teachers
        public ActionResult Index()
        {
            return View(db.Teachers.ToList());
        }

		//tim kiem giao vien
		[HttpPost]
		public ActionResult Index(FormCollection form)
		{
			//lay string nguoi dung nhap vao
			string keyword = form["keyword"].ToString();
			ViewBag.Keyword = keyword;
			//lay ra list giao vien co ten chua keyword nguoi dung vua nhap
			List<Teacher> listTeachersWithKeywords = db.Teachers.Where(x => x.TeacherName.Contains(keyword)).ToList();
			
			//neu ko tim thay ket qua
			if (listTeachersWithKeywords.Count == 0)
			{
				ViewBag.ErrorMessage = "Không tìm thấy giáo viên nào. Vui lòng thử lại";
				return View(db.Students.ToList());
			}
			else
			{
				ViewBag.SuccessMessage = "Danh sách giáo viên với từ khóa: " + keyword;
				ViewBag.Count = listTeachersWithKeywords.Count();
				return View(listTeachersWithKeywords.ToList());
			}
		}

		// GET: Teachers/Details/5
		public ActionResult Details(int? id)
        {
            if (id == null)
            {
				return RedirectToAction("Page404", "Authentication", new { area = "Authentication" });
			}
            Teacher teacher = db.Teachers.Find(id);
            if (teacher == null)
            {
				return RedirectToAction("Page404", "Authentication", new { area = "Authentication" });
			}
            return View(teacher);
        }

        // GET: Teachers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Teachers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TeacherName,Email,Phone,Office,Username,Password,Status,IsDeleted")] Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                db.Teachers.Add(teacher);
				int id = db.Teachers.Max(t => t.Id);
				User user = new User()
				{
					Username = teacher.Username,
					Password = teacher.Password,
					Position = "Teacher",
					TeacherId = id
				};
				db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(teacher);
        }

        // GET: Teachers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
				return RedirectToAction("Page404", "Authentication", new { area = "Authentication" });
			}
            Teacher teacher = db.Teachers.Find(id);
            if (teacher == null)
            {
				return RedirectToAction("Page404", "Authentication", new { area = "Authentication" });
			}
            return View(teacher);
        }

        // POST: Teachers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TeacherName,Email,Phone,Office,Username,Password,Status,IsDeleted")] Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                db.Entry(teacher).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(teacher);
        }

        // GET: Teachers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
				return RedirectToAction("Page404", "Authentication", new { area = "Authentication" });
			}
            Teacher teacher = db.Teachers.Find(id);
            if (teacher == null)
            {
				return RedirectToAction("Page404", "Authentication", new { area = "Authentication" });
			}
            return View(teacher);
        }

        // POST: Teachers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Teacher teacher = db.Teachers.Find(id);
			if(teacher == null)
			{
				return RedirectToAction("Page404", "Authentication", new { area = "Authentication" });
			}
			User user = db.Users.FirstOrDefault(u => u.TeacherId == teacher.Id);
            db.Teachers.Remove(teacher);
			db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

		[HttpPost]
		public ActionResult ImportTeacherFromExcel (HttpPostedFileBase file)
		{
			int count = 0;
			var package = new ExcelPackage(file.InputStream);
			if(ImportData(out count, package))
			{
				ViewBag.message = "Bạn đã import danh sách giáo viên thành công";
			}
			return RedirectToAction("Index", "Teachers", new { area = "Admin" });
		}

		public bool ImportData(out int count, ExcelPackage package)
		{
			var result = false;
			count = 0;
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
					object Name = worksheet.Cells[startRow, startColumn + 3].Value;
					object Email = worksheet.Cells[startRow, startColumn + 4].Value;

					if(data != null)
					{
						var isImported = SaveTeacher(Username.ToString(), Password.ToString(), 
							Name.ToString(), Email.ToString(), db);
						if(isImported)
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

		public bool SaveTeacher(string username, string password, string name, string email, ClassSurveyDbContext db)
		{
			var result = false;
			try
			{
				if(db.Teachers.Where(x => x.Username.Equals(username)).Count() == 0)
				{
					var teacher = new Teacher();
					teacher.Username = username;
					teacher.Password = password;
					teacher.TeacherName = name;
					teacher.Email = email;

					db.Teachers.Add(teacher);
					db.SaveChanges();

					int id = db.Teachers.Max(x => x.Id);
					User user = new User() {
						Username = username,
						Password = password,
						Position = "Teacher",
						TeacherId = id
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
