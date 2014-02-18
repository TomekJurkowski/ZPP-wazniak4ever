﻿using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WazniakWebsite.DAL;
using WazniakWebsite.Models;

namespace WazniakWebsite.Controllers
{
    public class TextAnswerController : Controller
    {
        private SchoolContext db = new SchoolContext();

        // GET: /TextAnswer/
        public ActionResult Index()
        {
            return View(db.TextAnswers.ToList());
        }

        // GET: /TextAnswer/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TextAnswer textanswer = db.TextAnswers.Find(id);
            if (textanswer == null)
            {
                return HttpNotFound();
            }
            return View(textanswer);
        }

        // GET: /TextAnswer/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /TextAnswer/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,answer")] TextAnswer textanswer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.TextAnswers.Add(textanswer);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            return View(textanswer);
        }

        // GET: /TextAnswer/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TextAnswer textanswer = db.TextAnswers.Find(id);
            if (textanswer == null)
            {
                return HttpNotFound();
            }
            return View(textanswer);
        }

        // POST: /TextAnswer/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,answer")] TextAnswer textanswer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(textanswer).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            return View(textanswer);
        }

        // GET: /TextAnswer/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TextAnswer textanswer = db.TextAnswers.Find(id);
            if (textanswer == null)
            {
                return HttpNotFound();
            }
            return View(textanswer);
        }

        // POST: /TextAnswer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TextAnswer textanswer = db.TextAnswers.Find(id);
            db.TextAnswers.Remove(textanswer);
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
