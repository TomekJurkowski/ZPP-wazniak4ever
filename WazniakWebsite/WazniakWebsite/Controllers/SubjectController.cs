using PagedList;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WazniakWebsite.DAL;
using WazniakWebsite.Models;

// The helper class below is used in this file in order to create a wrapped message
// that can be send as a result of a POST request, by ChangeModulesSequenceNumbers method.
// I consciously leave this class in this file.
namespace WazniakWebsite.Models
{
    [Serializable]
    public class Message
    {
        public string msg { get; set; }
    }
}


namespace WazniakWebsite.Controllers
{
    public class SubjectController : Controller
    {
        private SchoolContext db = new SchoolContext();

        // GET: /Subject/
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.SortParam = String.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var subjects = from s in db.Subjects
                           select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                subjects = subjects.Where(s => s.Name.ToUpper().Contains(searchString.ToUpper()));
            }

            // Put the results in correct order
            subjects = sortOrder == "Name_desc" ? subjects.OrderByDescending(s => s.Name) : subjects.OrderBy(s => s.Name);

            const int pageSize = 12;
            var pageNumber = (page ?? 1);

            return View(subjects.ToPagedList(pageNumber, pageSize));
        }

        // Private function creating a list of modules associated with the subject.
        private void PopulateModulesList(int? subjectId)
        {
            var modulesQuery = from m in db.Modules
                               where m.SubjectID == subjectId
                               orderby m.SequenceNo
                               select m;
            ViewBag.ModuleList = modulesQuery.ToList();
        }

        // GET: /Subject/Details/5
        public ActionResult Details(int? id, string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var subject = db.Subjects.Find(id);
            if (subject == null)
            {
                return HttpNotFound();
            }

            ViewBag.SingleValueAnswer = Answer.SINGLE_VALUE_ANSWER;
            ViewBag.TextAnswer = Answer.TEXT_ANSWER;
            ViewBag.SingleChoiceAnswer = Answer.SINGLE_CHOICE_ANSWER;
            ViewBag.MultipleChoiceAnswer = Answer.MULTIPLE_CHOICE_ANSWER;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.SortParam = String.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var tasks = from t in db.Tasks
                        where t.SubjectID == id
                        select t;

            if (!String.IsNullOrEmpty(searchString))
            {
                tasks = tasks.Where(s => s.Title.ToUpper().Contains(searchString.ToUpper()));
            }

            // Put the results in correct order
            tasks = sortOrder == "Name_desc" ? tasks.OrderByDescending(s => s.Title) : tasks.OrderBy(s => s.Title);

            const int pageSize = 12;
            var pageNumber = (page ?? 1);

            ViewBag.Tasks = tasks.ToPagedList(pageNumber, pageSize);

            PopulateModulesList(id);

            return View(subject);
        }

        [HttpPost]
        public ActionResult ChangeModulesSequenceNumbers(int subjectId, int[] moduleIds)
        {
            for (var i = 0; i < moduleIds.Length; ++i)
            {
                var module = db.Modules.Find(moduleIds[i]);
                if (module.SequenceNo != i)
                {
                    module.SequenceNo = i;
                    db.Entry(module).State = EntityState.Modified;
                }
            }

            var subject = db.Subjects.Find(subjectId);

            // Update subject time
            subject.UpdateLastUpdatedTime();
            db.Entry(subject).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new Message { msg = "The sequence of modules has been successfully changed!" },
                JsonRequestBehavior.DenyGet);
        }

        // GET: /Subject/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Subject/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,Name,Description")] Subject subject)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Update subject time
                    subject.UpdateLastUpdatedTime();
                    db.Subjects.Add(subject);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            return View(subject);
        }

        // GET: /Subject/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subject subject = db.Subjects.Find(id);
            if (subject == null)
            {
                return HttpNotFound();
            }
            return View(subject);
        }

        // POST: /Subject/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,Name,Description")] Subject subject)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Update subject time
                    subject.UpdateLastUpdatedTime();
                    db.Entry(subject).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            return View(subject);
        }

        // GET: /Subject/Delete/5
        public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            var subject = db.Subjects.Find(id);
            if (subject == null)
            {
                return HttpNotFound();
            }
            return View(subject);
        }

        // POST: /Subject/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                var subject = db.Subjects.Find(id);

                var arr = subject.Tasks.ToArray();
                // Remove all Tasks (with their Answers) that belong to the Subject
                for (var i = arr.Length - 1; i >= 0; --i)
                {
                    var taskId = arr[i].ID;

                    if (arr[i].Answer.className() == Answer.SINGLE_CHOICE_ANSWER)
                    {
                        var choices = db.SingleChoices.Where(choice => choice.SingleChoiceAnswerID == taskId).ToArray();
                        for (var j = choices.Length - 1; j >= 0; --j)
                        {
                            db.SingleChoices.Remove(choices[j]);                            
                        }
                    }
                    if (arr[i].Answer.className() == Answer.MULTIPLE_CHOICE_ANSWER)
                    {
                        var choices = db.MultiChoices.Where(choice => choice.MultipleChoiceAnswerID == taskId).ToArray();
                        for (var j = choices.Length - 1; j >= 0; --j)
                        {
                            db.MultiChoices.Remove(choices[j]);
                        }
                    }

                    db.Answers.Remove(arr[i].Answer);
                    db.Tasks.Remove(arr[i]);
                }

                db.Subjects.Remove(subject);
                db.SaveChanges();
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                return RedirectToAction("Delete", new { id, saveChangesError = true });
            }
            
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

        [HttpPost]
        public string GetModuleStatistics(int courseId)
        {
            var moduleStats = from module in db.Modules.Where(module => module.SubjectID == courseId)
                              select new
                              {
                                  id = module.ID,
                                  name = module.Title,
                                  y = module.ID,
                                  drilldown = true
                              };
            return Newtonsoft.Json.JsonConvert.SerializeObject(moduleStats);
        }

        [HttpPost]
        public string GetExerciseStatistics(int moduleId)
        {
            var iter = 0;
            var exerciseStats = (from exercise in db.Tasks.Where(task => task.ModuleID == moduleId)
                                 select new
                                 {
                                     name = exercise.Title,
                                     y = exercise.ID,
                                     id = exercise.ID
                                 }).ToList();
            return Newtonsoft.Json.JsonConvert.SerializeObject(exerciseStats);
        }

        [HttpPost]
        public string GetExerciseType(int exerciseId)
        {
            var firstOrDefault = db.Tasks.FirstOrDefault(task => task.ID == exerciseId);
            return firstOrDefault != null ? firstOrDefault.className() : null;
        }
    }
}
