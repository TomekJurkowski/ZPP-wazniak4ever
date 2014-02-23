using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WazniakWebsite.DAL;
using WazniakWebsite.Models;

namespace WazniakWebsite.Controllers
{
    public class RegularTaskController : Controller
    {
        // CONSTANTS representing what kind of Answer is the Task in relationship with
        private const string NO_ANSWER = "no_ans";
        private const string SINGLE_VALUE_ANSWER = "single_val_ans";
        private const string TEXT_ANSWER = "text_ans";
        private const string SINGLE_CHOICE_ANSWER = "single_cho_ans";
        private const string MULTIPLE_CHOICE_ANSWER = "multi_cho_ans";

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

        // GET: /RegularTask/Create/SubjectName/5
        public ActionResult Create(string subjectName, int subjectId)
        {
            ViewBag.SubjectName = subjectName;
            ViewBag.SubjectId = subjectId;

            ViewBag.NoAnswer = NO_ANSWER;
            ViewBag.SingleValueAnswer = SINGLE_VALUE_ANSWER;
            ViewBag.TextAnswer = TEXT_ANSWER;
            ViewBag.SingleChoiceAnswer = SINGLE_CHOICE_ANSWER;
            ViewBag.MultipleChoiceAnswer = MULTIPLE_CHOICE_ANSWER;

            return View();
        }

        // POST: /RegularTask/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,Title,Text")] RegularTask regulartask, string answerType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.RegularTasks.Add(regulartask);
                    switch (answerType)
                    {
                        case SINGLE_VALUE_ANSWER:

                            break;
                        case TEXT_ANSWER:

                            break;
                        case SINGLE_CHOICE_ANSWER:

                            break;
                        case MULTIPLE_CHOICE_ANSWER:

                            break;
                        default:
                            //TODO: error

                            break;
                    }

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
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(regulartask).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
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
