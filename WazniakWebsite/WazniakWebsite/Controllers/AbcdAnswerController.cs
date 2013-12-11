using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WazniakWebsite.Models;
using WazniakWebsite.DAL;

namespace WazniakWebsite.Controllers
{
    public class AbcdAnswerController : Controller
    {
        private SchoolContext db = new SchoolContext();

        // GET: /AbcdAnswer/
        public ActionResult Index()
        {
            return View(db.AbcdAnswers.ToList());
        }

        // GET: /AbcdAnswer/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AbcdAnswer abcdanswer = db.AbcdAnswers.Find(id);
            if (abcdanswer == null)
            {
                return HttpNotFound();
            }
            return View(abcdanswer);
        }

        // GET: /AbcdAnswer/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /AbcdAnswer/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,A,B,C,D,CorrectAnswer")] AbcdAnswer abcdanswer)
        {
            if (ModelState.IsValid)
            {
                db.AbcdAnswers.Add(abcdanswer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(abcdanswer);
        }

        // GET: /AbcdAnswer/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AbcdAnswer abcdanswer = db.AbcdAnswers.Find(id);
            if (abcdanswer == null)
            {
                return HttpNotFound();
            }
            return View(abcdanswer);
        }

        // POST: /AbcdAnswer/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,A,B,C,D,CorrectAnswer")] AbcdAnswer abcdanswer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(abcdanswer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(abcdanswer);
        }

        // GET: /AbcdAnswer/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AbcdAnswer abcdanswer = db.AbcdAnswers.Find(id);
            if (abcdanswer == null)
            {
                return HttpNotFound();
            }
            return View(abcdanswer);
        }

        // POST: /AbcdAnswer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AbcdAnswer abcdanswer = db.AbcdAnswers.Find(id);
            db.AbcdAnswers.Remove(abcdanswer);
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
