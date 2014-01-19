using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WazniakWebsite.Models;
using WazniakWebsite.DAL;

namespace WazniakWebsite.Controllers
{
    public class TextAnswerController : Controller
    {
        private SchoolContext db = new SchoolContext();
        //
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
            TextAnswer textAnswer = db.TextAnswers.Find(id);

            if (textAnswer == null)
            {
                return HttpNotFound();
            }
            return View(textAnswer);
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
        public ActionResult Create([Bind(Include = "ID,correctAnswer")] TextAnswer textAnswer)
        {
            if (ModelState.IsValid)
            {
                db.TextAnswers.Add(textAnswer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(textAnswer);
        }

        // GET: /TextAnswer/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TextAnswer textAnswer = db.TextAnswers.Find(id);
            if (textAnswer == null)
            {
                return HttpNotFound();
            }
            return View(textAnswer);
        }

        // POST: /TextAnswer/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,correctAnswer")] TextAnswer textAnswer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(textAnswer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(textAnswer);
        }

        // GET: /TextAnswer/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TextAnswer textAnswer = db.TextAnswers.Find(id);
            if (textAnswer == null)
            {
                return HttpNotFound();
            }
            return View(textAnswer);
        }

        // POST: /TextAnswer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TextAnswer textAnswer = db.TextAnswers.Find(id);
            db.TextAnswers.Remove(textAnswer);
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