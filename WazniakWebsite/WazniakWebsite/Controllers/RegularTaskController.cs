using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
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

        // CONSTANTS representing a statement that should be printed to the user if he has not filled the form properly
        private const string NO_ANSWER_PICKED_STATEMENT = "You must create an answer for your exercise!";
        private const string SINGLE_VALUE_STATEMENT = "You must provide a value for the Single Value Answer.";
        private const string TEXT_STATEMENT = "You must provide some text for the Text Answer.";

        private SchoolContext db = new SchoolContext();

        // GET: /RegularTask/
        //public ActionResult Index()
        //{
        //    return View(db.RegularTasks.ToList());
        //}

        // GET: /RegularTask/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var regulartask = db.RegularTasks.Find(id);
        
            if (regulartask == null)
            {
                return HttpNotFound();
            }
            return View(regulartask);
        }

        private void FillTheViewBag(string subjectName, int subjectId, string previousAns = NO_ANSWER)
        {
            ViewBag.SubjectName = subjectName;
            ViewBag.SubjectId = subjectId;

            ViewBag.NoAnswer = NO_ANSWER;
            ViewBag.SingleValueAnswer = SINGLE_VALUE_ANSWER;
            ViewBag.TextAnswer = TEXT_ANSWER;
            ViewBag.SingleChoiceAnswer = SINGLE_CHOICE_ANSWER;
            ViewBag.MultipleChoiceAnswer = MULTIPLE_CHOICE_ANSWER;

            ViewBag.PreviouslySelectedAnswer = previousAns;
        }

        // GET: /RegularTask/Create/SubjectName/5
        public ActionResult Create(string subjectName, int subjectId)
        {
            FillTheViewBag(subjectName, subjectId);

            return View();
        }

        // POST: /RegularTask/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,Text,SubjectID")] RegularTask regulartask, string subjectName, int subjectId,
            string answerType, string valueAns, string textAns)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    switch (answerType)
                    {
                        case SINGLE_VALUE_ANSWER:
                            if (String.IsNullOrEmpty(valueAns))
                            {
                                // Reload page with proper statement
                                ViewBag.SingleValueStatement = SINGLE_VALUE_STATEMENT;
                                FillTheViewBag(subjectName, subjectId, SINGLE_VALUE_ANSWER);

                                return View(regulartask);
                            }

                            // Create new SingleValueAnswer
                            var singleValueAnswer = new SingleValueAnswer(regulartask.ID, valueAns);
                            db.SingleValueAnswers.Add(singleValueAnswer);

                            db.RegularTasks.Add(regulartask);
                            break;
                        case TEXT_ANSWER:
                            if (String.IsNullOrEmpty(textAns))
                            {
                                // Reload page with proper statement
                                ViewBag.TextStatement = TEXT_STATEMENT;
                                FillTheViewBag(subjectName, subjectId, TEXT_ANSWER);

                                return View(regulartask);
                            }

                            // Create new TextAnswer
                            var textAnswer = new TextAnswer(regulartask.ID, textAns);
                            db.TextAnswers.Add(textAnswer);

                            db.RegularTasks.Add(regulartask);
                            break;
                        case SINGLE_CHOICE_ANSWER:



                            db.RegularTasks.Add(regulartask);
                            break;
                        case MULTIPLE_CHOICE_ANSWER:



                            db.RegularTasks.Add(regulartask);
                            break;
                        default:
                            // No answer has been selected - let's remind the user that he has to pick one
                            ViewBag.NoAnswerPickedStatement = NO_ANSWER_PICKED_STATEMENT;
                            FillTheViewBag(subjectName, subjectId);

                            return View(regulartask);
                    }

                    db.SaveChanges();
                    return RedirectToAction("Details", "Subject", new { id = subjectId });
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("",
                    "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            FillTheViewBag(subjectName, subjectId);
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
            var regulartask = db.RegularTasks.Find(id);
            var x = regulartask.Answer;
            db.Answers.Remove(regulartask.Answer);
            db.RegularTasks.Remove(regulartask);

            db.SaveChanges();
            return RedirectToAction("Index", "Subject");
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
