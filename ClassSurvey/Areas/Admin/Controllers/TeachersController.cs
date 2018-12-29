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

		// GET: Teachers/Details/5
		public ActionResult Details(int? id)
        {
			//not found id
            if (id == null)
            {
				return RedirectToAction("Page404", "Authentication", new { area = "Authentication" });
			}
            Teacher teacher = db.Teachers.Find(id);

			//not found teacher
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
				//hash password before adding
				teacher.Password = HashPassword.ComputeSha256Hash(teacher.Password);
				db.Teachers.Add(teacher);
				db.SaveChanges();
				int id = db.Teachers.Max(t => t.Id);
				//add new user
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
				teacher.Password = HashPassword.ComputeSha256Hash(teacher.Password);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(teacher);
        }

        // GET: Teachers/Delete/5
        public ActionResult Delete(int? id)
        {
			//not found id
            if (id == null)
            {
				return RedirectToAction("Page404", "Authentication", new { area = "Authentication" });
			}
            Teacher teacher = db.Teachers.Find(id);
			//not found teacher
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
			//remove teacher as well as user
            db.Teachers.Remove(teacher);
			db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

		//import teacher from file xlsx
		//method HttpPost
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

		//get data for above function
		public bool ImportData(out int count, ExcelPackage package)
		{
			var result = false;
			count = 0;
			try
			{
				//teacher info start at column 1 and row 2 in file excel
				int startColumn = 1;
				int startRow = 2;
				ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
				object data = null;
				ClassSurveyDbContext db = new ClassSurveyDbContext();

				do
				{
					data = worksheet.Cells[startRow, startColumn].Value;
					//get username
					object Username = worksheet.Cells[startRow, startColumn + 1].Value;
					//get password
					object Password = worksheet.Cells[startRow, startColumn + 2].Value;
					//get teacher name
					object Name = worksheet.Cells[startRow, startColumn + 3].Value;
					//get email
					object Email = worksheet.Cells[startRow, startColumn + 4].Value;

					//if exists data
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

		//check if we can save new teachers
		public bool SaveTeacher(string username, string password, string name, string email, ClassSurveyDbContext db)
		{
			var result = false;
			try
			{
				//if teacher not exists in system before
				//just add new teachers when they dont exist in system 
				if(db.Teachers.Where(x => x.Username.Equals(username)).Count() == 0)
				{
					var teacher = new Teacher();
					teacher.Username = username;
					//hash password before adding
					teacher.Password = HashPassword.ComputeSha256Hash(password);
					teacher.TeacherName = name;
					teacher.Email = email;

					db.Teachers.Add(teacher);
					db.SaveChanges();

					int id = db.Teachers.Max(x => x.Id);
					//add new user
					User user = new User() {
						Username = username,
						Password = HashPassword.ComputeSha256Hash(password),
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
