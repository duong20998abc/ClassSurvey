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

namespace ClassSurvey.Areas.Admin.Controllers
{
	[AuthorizeBusiness]
    public class SurveysController : Controller
    {
        private ClassSurveyDbContext db = new ClassSurveyDbContext();

        // GET: Surveys
        public ActionResult Index()
        {
            return View(db.SurveyQuestions.ToList());
        }

		public ActionResult Create()
		{
			return View();
		}

		// POST: Surveys/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Content,Status,IsDeleted")] SurveyQuestion surveyQuestion)
        {
			//1 ham trong Entity Framework
			//Disable create Proxy when create a new context
			//useful in lazy loading
		
			if (ModelState.IsValid)
			{
				//Tao moi 1 cau hoi trong danh sach cau hoi cua survey va add vao DB
				db.SurveyQuestions.Add(surveyQuestion);
				db.SaveChanges();
				//Xoa bo diem danh gia theo tieu chi cu => Yeu cau sinh vien cap nhat ban danh gia khac voi noi dung moi
				db.Surveys.RemoveRange(db.Surveys.ToList());
				db.SaveChanges();

				return RedirectToAction("Index", "Surveys", new { area = "Admin" });
			}

			return View(surveyQuestion);
        }

        // GET: Surveys/Edit/5
        public ActionResult Edit(int? id)
        {
			//Khong tim thay id
            if (id == null)
            {
				return RedirectToAction("Page404", "Authentication", new { area = "Authentication" });
			}

			//Tim cau hoi co id trung voi Id 
			SurveyQuestion question = db.SurveyQuestions.FirstOrDefault(q => q.Id == id);

			//Khong tim thay cau hoi
            if (question == null)
            {
				return RedirectToAction("Page404", "Authentication", new { area = "Authentication" });
			}
            return View(question);
        }

        // POST: Surveys/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Content,Status,IsDeleted")] SurveyQuestion surveyQuestion)
        {
			if (ModelState.IsValid)
			{
				db.Entry(surveyQuestion).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Index", "Surveys", new { area = "Admin" });
			}
			return View(surveyQuestion);
		}

        // GET: Surveys/Delete/5
        public ActionResult Delete(int? id)
        {
			db.Configuration.ProxyCreationEnabled = false;
			if (id == null)
            {
				return RedirectToAction("Page404", "Authentication", new { area = "Authentication" });
			}
            SurveyQuestion survey = db.SurveyQuestions.FirstOrDefault(q => q.Id == id);
            if (survey == null)
            {
				return RedirectToAction("Page404", "Authentication", new { area = "Authentication" });
			}
            return View(survey);
        }

        // POST: Surveys/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SurveyQuestion question = db.SurveyQuestions.Find(id);
			if(question == null)
			{
				return RedirectToAction("Page404", "Authentication", new { area = "Authentication" });
			}

			db.SurveyQuestions.Remove(question);

			//lay sinh vien da danh gia cau hoi do va xoa di vi du lieu da thay doi
			IEnumerable<Survey> listSurveys = db.Surveys.Where(s => s.SurveyQuestionId == id);
			db.Surveys.RemoveRange(listSurveys);
            db.SaveChanges();
			return RedirectToAction("Index", "Surveys", new { area = "Admin" });
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
