﻿using System;
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
        // CONSTANTS representing a statement that should be printed to the user if he has not filled the form properly
        private const string NO_ANSWER_PICKED_STATEMENT = "You must create an answer for your exercise!";
        private const string SINGLE_VALUE_STATEMENT = "You must provide a value for the Single Value Answer.";
        private const string TEXT_STATEMENT = "You must provide some text for the Text Answer.";

        private SchoolContext db = new SchoolContext();

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
                        case Answer.SINGLE_VALUE_ANSWER:
                            if (String.IsNullOrEmpty(valueAns))
                            {
                                // Reload page with proper statement
                                ViewBag.SingleValueStatement = SINGLE_VALUE_STATEMENT;
                                FillTheViewBag(subjectName, subjectId, Answer.SINGLE_VALUE_ANSWER);

                                return View(regulartask);
                            }

                            // Create new SingleValueAnswer
                            var singleValueAnswer = new SingleValueAnswer(regulartask.ID, valueAns);
                            db.SingleValueAnswers.Add(singleValueAnswer);
                            break;
                        case Answer.TEXT_ANSWER:
                            if (String.IsNullOrEmpty(textAns))
                            {
                                // Reload page with proper statement
                                ViewBag.TextStatement = TEXT_STATEMENT;
                                FillTheViewBag(subjectName, subjectId, Answer.TEXT_ANSWER);

                                return View(regulartask);
                            }

                            // Create new TextAnswer
                            var textAnswer = new TextAnswer(regulartask.ID, textAns);
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

                            return View(regulartask);
                    }

                    // Update subject time
                    var sub = db.Subjects.Find(regulartask.SubjectID);
                    sub.UpdateLastUpdatedTime();
                    db.Entry(sub).State = EntityState.Modified;

                    db.RegularTasks.Add(regulartask);
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

            var regulartask = db.RegularTasks.Find(id);
            if (regulartask == null)
            {
                return HttpNotFound();
            }

            var sub = db.Subjects.Find(regulartask.SubjectID);

            // Edit view does not really require the first two arguments to be passed into ViewBag,
            // but it might in the future (probably won't) plus is is easier to use FillTheViewBag function.
            FillTheViewBag(sub.Name, sub.ID, regulartask.Answer.className());

            return View(regulartask);
        }

        // POST: /RegularTask/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,Text,SubjectID,SubjectID")] RegularTask regulartask, int isAnswerChanged, string answerType,
            string valueAns, string textAns)
        {
            var sub = db.Subjects.Find(regulartask.SubjectID);

            try
            {
                if (ModelState.IsValid)
                {
                    // First let's see if the answer for the task is being changed or not.
                    // If it's not then we have an easy case to deal with
                    if (isAnswerChanged == 0)
                    {
                        // Update subject time
                        sub.UpdateLastUpdatedTime();
                        db.Entry(sub).State = EntityState.Modified;

                        db.Entry(regulartask).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Details", "Subject", new { id = regulartask.SubjectID });                        
                    }

                    // Variable holding answer that belongs to the task
                    var ans = db.Answers.Find(regulartask.ID);

                    // And now let's cope with the case when the answer is being changed as well as the task.
                    switch (answerType)
                    {
                        case Answer.SINGLE_VALUE_ANSWER:
                            if (String.IsNullOrEmpty(valueAns))
                            {
                                // Reload page with proper statement
                                ViewBag.SingleValueStatement = SINGLE_VALUE_STATEMENT;
                                @ViewBag.ReloadPage = 1;
                                FillTheViewBag(sub.Name, sub.ID, Answer.SINGLE_VALUE_ANSWER);
                                
                                // In this case we want to reload the Edit page with the original RegularTask
                                return View(db.RegularTasks.Find(regulartask.ID));
                            }

                            if (ans.className() == answerType)
                            {
                                // We don't have to create a completely new Answer, only update the current one
                                ((SingleValueAnswer)ans).Value = valueAns;
                                db.Entry(ans).State = EntityState.Modified;
                            }
                            else
                            {
                                // Delete previous answer and create new SingleValueAnswer
                                db.Answers.Remove(ans);
                                db.SaveChanges();

                                var singleValueAnswer = new SingleValueAnswer(regulartask.ID, valueAns);
                                db.SingleValueAnswers.Add(singleValueAnswer);
                            }

                            break;
                        case Answer.TEXT_ANSWER:
                            if (String.IsNullOrEmpty(textAns))
                            {
                                // Reload page with proper statement
                                ViewBag.TextStatement = TEXT_STATEMENT;
                                @ViewBag.ReloadPage = 1;
                                FillTheViewBag(sub.Name, sub.ID, Answer.TEXT_ANSWER);

                                // In this case we want to reload the Edit page with the original RegularTask
                                return View(db.RegularTasks.Find(regulartask.ID));
                            }

                            if (ans.className() == answerType)
                            {
                                // We don't have to create a completely new Answer, only update the current one
                                ((TextAnswer)ans).Text = textAns;
                                db.Entry(ans).State = EntityState.Modified;
                            }
                            else
                            {
                                // Delete previous answer and create new TextAnswer
                                db.Answers.Remove(ans);
                                db.SaveChanges();

                                var textAnswer = new TextAnswer(regulartask.ID, textAns);
                                db.TextAnswers.Add(textAnswer);      
                            }

                            break;
                        case Answer.SINGLE_CHOICE_ANSWER:

                            if (regulartask.Answer.className() == answerType)
                            {
                                // We don't have to create a completely new Answer, only update the current one
                            
                            }
                            else
                            {
                                
                            }

                            break;
                        case Answer.MULTIPLE_CHOICE_ANSWER:

                            if (regulartask.Answer.className() == answerType)
                            {
                                // We don't have to create a completely new Answer, only update the current one

                            }
                            else
                            {
                                
                            }

                            break;
                    }

                    // Update subject time
                    sub.UpdateLastUpdatedTime();
                    db.Entry(sub).State = EntityState.Modified;

                    db.Entry(regulartask).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Details", "Subject", new { id = regulartask.SubjectID });
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            // Edit view does not really require the first two arguments to be passed into ViewBag,
            // but it might in the future (probably won't) plus is is easier to use FillTheViewBag function.
            FillTheViewBag(sub.Name, sub.ID, regulartask.Answer.className());

            // In this case we want to reload the Edit page with the original RegularTask
            return View(db.RegularTasks.Find(regulartask.ID));
        }

        // GET: /RegularTask/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: /RegularTask/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var regulartask = db.RegularTasks.Find(id);
            var subjectId = regulartask.SubjectID;

            try
            {
                // Update subject time
                var sub = db.Subjects.Find(regulartask.SubjectID);
                sub.UpdateLastUpdatedTime();
                db.Entry(sub).State = EntityState.Modified;

                db.Answers.Remove(regulartask.Answer);
                db.RegularTasks.Remove(regulartask);

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
