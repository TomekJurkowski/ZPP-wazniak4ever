using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WazniakWebsite.Models;
using WazniakWebsite.DAL;

namespace WazniakWebsite.Controllers
{
    public class RegularTaskController : Controller
    {
        private SchoolContext db = new SchoolContext();

        // GET: /RegularTask/
        public ActionResult Index()
        {
            return View(db.RegularTasks.ToList());
        }

        // GET: /RegularTask/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RegularTask regulartask = db.RegularTasks.Find(id);
            if (regulartask == null)
            {
                return HttpNotFound();
            }
            return View(regulartask);
        }

        // GET: /RegularTask/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /RegularTask/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,Title,Text")] RegularTask regulartask)
        {
            if (ModelState.IsValid)
            {
                db.RegularTasks.Add(regulartask);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(regulartask);
        }

        // GET: /RegularTask/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RegularTask regulartask = db.RegularTasks.Find(id);
            if (regulartask == null)
            {
                return HttpNotFound();
            }
            return View(regulartask);
        }

        // POST: /RegularTask/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,Title,Text")] RegularTask regulartask)
        {
            if (ModelState.IsValid)
            {
                db.Entry(regulartask).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(regulartask);
        }

        // GET: /RegularTask/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RegularTask regulartask = db.RegularTasks.Find(id);
            if (regulartask == null)
            {
                return HttpNotFound();
            }
            return View(regulartask);
        }

        // POST: /RegularTask/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RegularTask regulartask = db.RegularTasks.Find(id);
            db.RegularTasks.Remove(regulartask);
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
