using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Net;
using System.Web.Mvc;
using WazniakWebsite.DAL;
using WazniakWebsite.Models;

namespace WazniakWebsite.Controllers
{
    public class MathematicalTaskController : Controller
    {
        // CONSTANTS representing a statement that should be printed to the user if he has not filled the form properly
        private const string NO_ANSWER_PICKED_STATEMENT = "You must create an answer for your exercise!";
        private const string SINGLE_VALUE_STATEMENT = "You must provide a value for the Single Value Answer.";
        private const string TEXT_STATEMENT = "You must provide some text for the Text Answer.";

        private SchoolContext db = new SchoolContext();

        // GET: /MathematicalTask/
        //public ActionResult Index()
        //{
        //    return View(db.MathematicalTasks.ToList());
        //}

        // GET: /MathematicalTask/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var mathematicaltask = db.MathematicalTasks.Find(id);
            
            if (mathematicaltask == null)
            {
                return HttpNotFound();
            }
            return View(mathematicaltask);
        }

        private void FillTheViewBag(string subjectName, int subjectId, string previousAns = Answer.NO_ANSWER)
        {
            ViewBag.SubjectName = subjectName;
            ViewBag.SubjectId = subjectId;

            ViewBag.NoAnswer = Answer.NO_ANSWER;
            ViewBag.SingleValueAnswer = Answer.SINGLE_VALUE_ANSWER;
            ViewBag.TextAnswer = Answer.TEXT_ANSWER;
            ViewBag.SingleChoiceAnswer = Answer.SINGLE_CHOICE_ANSWER;
            ViewBag.MultipleChoiceAnswer = Answer.MULTIPLE_CHOICE_ANSWER;

            ViewBag.PreviouslySelectedAnswer = previousAns;
        }

        // GET: /MathematicalTask/Create//SubjectName/5
        public ActionResult Create(string subjectName, int subjectId)
        {
            FillTheViewBag(subjectName, subjectId);

            return View();
        }

        // POST: /MathematicalTask/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,Text,SubjectID")] MathematicalTask mathematicaltask, string subjectName, int subjectId,
            string answerType, string valueAns, string textAns)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    switch (answerType)
                    {
                        case Answer.SINGLE_VALUE_ANSWER:
                            if (String.IsNullOrEmpty(valueAns))
                            {
                                // Reload page with proper statement
                                ViewBag.SingleValueStatement = SINGLE_VALUE_STATEMENT;
                                FillTheViewBag(subjectName, subjectId, Answer.SINGLE_VALUE_ANSWER);

                                return View(mathematicaltask);
                            }

                            // Create new SingleValueAnswer
                            var singleValueAnswer = new SingleValueAnswer(mathematicaltask.ID, valueAns);
                            db.SingleValueAnswers.Add(singleValueAnswer);
                            break;
                        case Answer.TEXT_ANSWER:
                            if (String.IsNullOrEmpty(textAns))
                            {
                                // Reload page with proper statement
                                ViewBag.TextStatement = TEXT_STATEMENT;
                                FillTheViewBag(subjectName, subjectId, Answer.TEXT_ANSWER);

                                return View(mathematicaltask);
                            }

                            // Create new TextAnswer
                            var textAnswer = new TextAnswer(mathematicaltask.ID, textAns);
                            db.TextAnswers.Add(textAnswer);
                            break;
                        case Answer.SINGLE_CHOICE_ANSWER:



                            break;
                        case Answer.MULTIPLE_CHOICE_ANSWER:



                            break;
                        default:
                            // No answer has been selected - let's remind the user that he has to pick one
                            ViewBag.NoAnswerPickedStatement = NO_ANSWER_PICKED_STATEMENT;
                            FillTheViewBag(subjectName, subjectId);

                            return View(mathematicaltask);
                    }

                    // Update subject time
                    var sub = db.Subjects.Find(mathematicaltask.SubjectID);
                    sub.UpdateLastUpdatedTime();
                    db.Entry(sub).State = EntityState.Modified;

                    db.MathematicalTasks.Add(mathematicaltask);
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
            return View(mathematicaltask);
        }

        // GET: /MathematicalTask/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var mathematicaltask = db.MathematicalTasks.Find(id);
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
        public ActionResult Edit([Bind(Include = "ID,Title,Text,SubjectID")] MathematicalTask mathematicaltask)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Update subject time
                    var sub = db.Subjects.Find(mathematicaltask.SubjectID);
                    sub.UpdateLastUpdatedTime();
                    db.Entry(sub).State = EntityState.Modified;

                    db.Entry(mathematicaltask).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Details", "Subject", new { id = mathematicaltask.SubjectID });
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

            var mathematicaltask = db.MathematicalTasks.Find(id);
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
            var mathematicaltask = db.MathematicalTasks.Find(id);
            var subjectId = mathematicaltask.SubjectID;

            try
            {
                // Update subject time
                var sub = db.Subjects.Find(mathematicaltask.SubjectID);
                sub.UpdateLastUpdatedTime();
                db.Entry(sub).State = EntityState.Modified;

                db.Answers.Remove(mathematicaltask.Answer);
                db.MathematicalTasks.Remove(mathematicaltask);

                db.SaveChanges();
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            return RedirectToAction("Details", "Subject", new { id = subjectId });
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
