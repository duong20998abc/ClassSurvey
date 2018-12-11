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

namespace ClassSurvey.Controllers
{
    public class SurveysController : Controller
    {
        private ClassSurveyDbContext db = new ClassSurveyDbContext();

        // GET: Surveys
        public ActionResult Index()
        {
            return View(db.SurveyQuestions.ToList());
        }

        // GET: Surveys/Create
        //public ActionResult Create()
        //{
        //    ViewBag.StudentClassId = new SelectList(db.StudentClasses, "Id", "Id");
        //    ViewBag.SurveyQuestionId = new SelectList(db.SurveyQuestions, "Id", "Content");
        //    return View();
        //}

        // POST: Surveys/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FormCollection form)
        {
			//1 ham trong Entity Framework
			//Disable create Proxy when create a new context
			//useful in lazy loading
			db.Configuration.ProxyCreationEnabled = false;

			string content = form["Content"].ToString();
			if (!db.SurveyQuestions.Any(s => s.Content.Equals(content)))
			{
				//Tao moi 1 cau hoi trong danh sach cau hoi cua survey va add vao DB
				SurveyQuestion question = new SurveyQuestion();
				question.Content = content;
				db.SurveyQuestions.Add(question);
				//Xoa bo diem danh gia theo tieu chi cu => Yeu cau sinh vien cap nhat ban danh gia khac voi noi dung moi
				db.Surveys.RemoveRange(db.Surveys.ToList());
				db.SaveChanges();

				//Lay ra id cua cau hoi trong survey vua duoc tao
				int NewQuestionId = db.SurveyQuestions.Max(q => q.Id);

				//dat status 1 cau hoi moi duoc tao la Create, lay ra Json bao gom status, content, id
				return Json(new {status = "Success", content = content, id = NewQuestionId }, JsonRequestBehavior.AllowGet);
			}else
			{
				//default cua HttpPost la DenyGet de ngan specific attack
				//o day ta muon su dung hoac allow Get, ta su dung JsonRequestBehavior.AllowGet
				return Json(new { status = "Fail" }, JsonRequestBehavior.AllowGet);
			}
        }

        // GET: Surveys/Edit/5
        public ActionResult Edit(int? id)
        {
			//Khong tim thay id
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

			//Tim cau hoi co id trung voi Id 
			SurveyQuestion question = db.SurveyQuestions.FirstOrDefault(q => q.Id == id);

			//Khong tim thay cau hoi
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        // POST: Surveys/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FormCollection form)
        {
			db.Configuration.ProxyCreationEnabled = false;
			string content = form["Content"].ToString();
			int QuestionId = int.Parse(form["QuestionId"].ToString());
			
			if(db.SurveyQuestions.Any(q => q.Content.Equals(content) && q.Id != QuestionId))
			{
				return Json(new { status = "Fail" }, JsonRequestBehavior.AllowGet);
			}else
			{
				SurveyQuestion surveyQuestion = db.SurveyQuestions.FirstOrDefault(q => q.Id == QuestionId);
				surveyQuestion.Content = content;
				db.SaveChanges();
				return Json(new { status = "Edited" }, JsonRequestBehavior.AllowGet);
			}
        }

        // GET: Surveys/Delete/5
        public ActionResult Delete(int? id)
        {
			db.Configuration.ProxyCreationEnabled = false;
			if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SurveyQuestion survey = db.SurveyQuestions.FirstOrDefault(q => q.Id == id);
            if (survey == null)
            {
                return HttpNotFound();
            }
            return Json(survey, JsonRequestBehavior.AllowGet);
        }

        // POST: Surveys/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SurveyQuestion question = db.SurveyQuestions.Find(id);
			if(question == null)
			{
				return HttpNotFound();
			}

			db.SurveyQuestions.Remove(question);

			//lay sinh vien da danh gia cau hoi do va xoa di vi du lieu da thay doi
			IEnumerable<Survey> listSurveys = db.Surveys.Where(s => s.SurveyQuestionId == id);
			db.Surveys.RemoveRange(listSurveys);
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
