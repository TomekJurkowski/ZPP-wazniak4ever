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
    public class SampleItemController : Controller
    {
        private SchoolContext db = new SchoolContext();

        // GET: /SampleItem/
        public ActionResult Index()
        {
            return View(db.SampleItems.ToList());
        }

        // GET: /SampleItem/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SampleItem sampleitem = db.SampleItems.Find(id);
            if (sampleitem == null)
            {
                return HttpNotFound();
            }
            return View(sampleitem);
        }

        // GET: /SampleItem/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /SampleItem/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,Text,Number")] SampleItem sampleitem)
        {
            if (ModelState.IsValid)
            {
                db.SampleItems.Add(sampleitem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(sampleitem);
        }

        // GET: /SampleItem/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SampleItem sampleitem = db.SampleItems.Find(id);
            if (sampleitem == null)
            {
                return HttpNotFound();
            }
            return View(sampleitem);
        }

        // POST: /SampleItem/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,Text,Number")] SampleItem sampleitem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sampleitem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sampleitem);
        }

        // GET: /SampleItem/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SampleItem sampleitem = db.SampleItems.Find(id);
            if (sampleitem == null)
            {
                return HttpNotFound();
            }
            return View(sampleitem);
        }

        // POST: /SampleItem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SampleItem sampleitem = db.SampleItems.Find(id);
            db.SampleItems.Remove(sampleitem);
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
