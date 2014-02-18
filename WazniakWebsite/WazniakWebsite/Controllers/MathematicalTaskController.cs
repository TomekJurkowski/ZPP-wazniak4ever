﻿using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WazniakWebsite.DAL;
using WazniakWebsite.Models;

namespace WazniakWebsite.Controllers
{
    public class MathematicalTaskController : Controller
    {
        private SchoolContext db = new SchoolContext();

        // GET: /MathematicalTask/
        public ActionResult Index()
        {
            return View(db.MathematicalTasks.ToList());
        }

        // GET: /MathematicalTask/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MathematicalTask mathematicaltask = db.MathematicalTasks.Find(id);
            if (mathematicaltask == null)
            {
                return HttpNotFound();
            }
            return View(mathematicaltask);
        }

        // GET: /MathematicalTask/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /MathematicalTask/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,Title,Text")] MathematicalTask mathematicaltask)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.MathematicalTasks.Add(mathematicaltask);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("",
                    "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            return View(mathematicaltask);
        }

        // GET: /MathematicalTask/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MathematicalTask mathematicaltask = db.MathematicalTasks.Find(id);
            if (mathematicaltask == null)
            {
                return HttpNotFound();
            }
            return View(mathematicaltask);
        }

        // POST: /MathematicalTask/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,Text")] MathematicalTask mathematicaltask)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(mathematicaltask).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            return View(mathematicaltask);
        }

        // GET: /MathematicalTask/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MathematicalTask mathematicaltask = db.MathematicalTasks.Find(id);
            if (mathematicaltask == null)
            {
                return HttpNotFound();
            }
            return View(mathematicaltask);
        }

        // POST: /MathematicalTask/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MathematicalTask mathematicaltask = db.MathematicalTasks.Find(id);
            db.MathematicalTasks.Remove(mathematicaltask);
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
