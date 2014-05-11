using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
        private const string MULTI_STATEMENT = "You must provide at least 1 (preferably at least 3) statements for Multiple Choice Answer.";
        private const string SINGLE_STATEMENT = "You must provide at least 1 (preferably at least 3) statements for Single Choice Answer and none of the statements can be empty.";

        private SchoolContext db = new SchoolContext();
        private BlobStorageService _blobStorageService = new BlobStorageService();

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

        // Private function responsible for filling the ViewBag with proper values.
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,Title,Text,SubjectID")] MathematicalTask mathematicaltask,
            string subjectName, int subjectId, string answerType, string valueAns, string textAns,
            string[] multiChoiceList, string[] multiAnswerList, string[] singleChoiceList, int singleCorrectNo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return await CreateTaskInternal(mathematicaltask, subjectName, subjectId, answerType, valueAns,
                        textAns, multiChoiceList, multiAnswerList, singleChoiceList, singleCorrectNo);
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

        // Private function used by POST Create method. It is the major part of that method,
        // since it is responsible for the actual creation of a new MathematicalTask with its Answer
        // and inserting those objects into the database. This function returns a proper ActionResult
        // depending whether the creation/insertion was successful or failed at some point.
        private async Task<ActionResult> CreateTaskInternal(MathematicalTask mathematicaltask, string subjectName,
            int subjectId, string answerType, string valueAns, string textAns, IList<string> multiChoiceList,
            IList<string> multiAnswerList, ICollection<string> singleChoiceList, int singleCorrectNo)
        {
            switch (answerType)
            {
                case Answer.SINGLE_VALUE_ANSWER:
                    if (String.IsNullOrEmpty(valueAns))
                    {
                        return ReloadPageWithStatement(mathematicaltask, answerType, subjectName, subjectId);
                    }

                    // Create new SingleValueAnswer
                    var singleValueAnswer = new SingleValueAnswer(valueAns);
                    db.SingleValueAnswers.Add(singleValueAnswer);
                    
                    break;
                case Answer.TEXT_ANSWER:
                    if (String.IsNullOrEmpty(textAns))
                    {
                        return ReloadPageWithStatement(mathematicaltask, answerType, subjectName, subjectId);
                    }

                    // Create new TextAnswer
                    var textAnswer = new TextAnswer(textAns);
                    db.TextAnswers.Add(textAnswer);

                    break;
                case Answer.SINGLE_CHOICE_ANSWER:
                    if (singleCorrectNo >= singleChoiceList.Count || singleCorrectNo < 0)
                    {
                        throw new RetryLimitExceededException("The number of the correct Choice for SingleChoiceAnswer is beyond valid range.");
                    }

                    // Case when we haven't got any valid Choices for the SingleChoiceAnswer or some choices are empty
                    if (singleChoiceList.Count == 0 || singleChoiceList.Count(String.IsNullOrEmpty) != 0)
                    {
                        return ReloadPageWithStatement(mathematicaltask, answerType, subjectName, subjectId);
                    }

                    // Create new SingleChoiceAnswer
                    var singleChoiceAnswer = new SingleChoiceAnswer(singleCorrectNo);
                    db.SingleChoiceAnswers.Add(singleChoiceAnswer);

                    foreach (var singleChoice in singleChoiceList.Select(s => new SingleChoice(s)))
                    {
                        db.SingleChoices.Add(singleChoice);
                    }

                    break;
                case Answer.MULTIPLE_CHOICE_ANSWER:
                    if (multiChoiceList.Count != multiAnswerList.Count)
                    {
                        throw new RetryLimitExceededException("The number of Options doesn't equal the number of true-false answers.");
                    }

                    // Case when we haven't got any valid Choices for the MultiChoiceAnswer
                    if (multiChoiceList.Count(t => !String.IsNullOrEmpty(t)) == 0)
                    {
                        return ReloadPageWithStatement(mathematicaltask, answerType, subjectName, subjectId);
                    }

                    // Create new MultiChoiceAnswer
                    var multiChoiceAnswer = new MultipleChoiceAnswer();
                    db.MultipleChoiceAnswers.Add(multiChoiceAnswer);

                    for (var i = 0; i < multiChoiceList.Count; i++)
                    {
                        // No safety check whether the values send by POST are valid ones ('True' and 'False' are valid).
                        // Every invalid value will be interpreted as 'False'
                        var tempAns = (multiAnswerList[i].Equals("True", StringComparison.OrdinalIgnoreCase));
                        var multiChoice = new MultiChoice(multiChoiceList[i], tempAns);
                        db.MultiChoices.Add(multiChoice);
                    }

                    break;
                default:
                    // No answer has been selected - let's remind the user that he has to pick one
                    return ReloadPageWithStatement(mathematicaltask, answerType, subjectName, subjectId);
            }

            UpdateSubjectTime(db.Subjects.Find(mathematicaltask.SubjectID));

            mathematicaltask.CorrectAnswers = 0;
            mathematicaltask.Attempts = 0;

            db.MathematicalTasks.Add(mathematicaltask);
            db.SaveChanges();

            mathematicaltask.ModifiedText = await UploadImagesToBlob(mathematicaltask.Text, mathematicaltask.ID);
            db.Entry(mathematicaltask).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Details", "Subject", new { id = subjectId });
        }

        // This function is called, when user does not provide us with all the necessary
        // data, that is needed to create/edit a task. It basically puts a proper statement
        // for the user into the ViewBag and returns an ActionResult that will display
        // the Create/Edit page once again with those statements.
        private ActionResult ReloadPageWithStatement(MathematicalTask mathematicaltask, string answerType,
            string subjectName, int subjectId)
        {
            switch (answerType)
            {
                case Answer.SINGLE_VALUE_ANSWER:
                    ViewBag.SingleValueStatement = SINGLE_VALUE_STATEMENT;
                    break;
                case Answer.TEXT_ANSWER:
                    ViewBag.TextStatement = TEXT_STATEMENT;
                    break;
                case Answer.SINGLE_CHOICE_ANSWER:
                    ViewBag.SingleChoiceStatement = SINGLE_STATEMENT;
                    break;
                case Answer.MULTIPLE_CHOICE_ANSWER:
                    ViewBag.MultiChoiceStatement = MULTI_STATEMENT;
                    break;
                default:
                    ViewBag.NoAnswerPickedStatement = NO_ANSWER_PICKED_STATEMENT;
                    break;
            }

            // Create Page does not use ReloadPage, but it's easier to leave it like that,
            // because it does not damage that page
            ViewBag.ReloadPage = 1;
            FillTheViewBag(subjectName, subjectId, answerType);

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

            var sub = db.Subjects.Find(mathematicaltask.SubjectID);

            // Edit view does not really require the first two arguments to be passed into ViewBag,
            // but it might in the future (probably won't) plus is is easier to use FillTheViewBag function.
            FillTheViewBag(sub.Name, sub.ID, mathematicaltask.Answer.className());

            return View(mathematicaltask);
        }

        // POST: /MathematicalTask/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,Title,Text,SubjectID")] MathematicalTask mathematicaltask,
            int isAnswerChanged, string answerType, string valueAns, string textAns, string[] multiChoiceList,
            string[] multiAnswerList, string[] singleChoiceList, int singleCorrectNo)
        {
            var sub = db.Subjects.Find(mathematicaltask.SubjectID);

            try
            {
                if (ModelState.IsValid)
                {
                    return await EditTaskInternal(mathematicaltask, sub, isAnswerChanged, answerType, valueAns,
                        textAns, multiChoiceList, multiAnswerList, singleChoiceList, singleCorrectNo);
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            // Edit view does not really require the first two arguments to be passed into ViewBag,
            // but it might in the future (probably won't) plus is is easier to use FillTheViewBag function.
            FillTheViewBag(sub.Name, sub.ID, db.Answers.Find(mathematicaltask.ID).className());

            // In this case we want to reload the Edit page with the original MathematicalTask
            return View(mathematicaltask);
        }

        // Private function used by POST Edit method. It is the major part of that method,
        // since it is responsible for the actual edition of an existing MathematicalTask (and its Answer).
        // This function returns a proper ActionResult depending whether the action was successful 
        // or failed at some point.
        private async Task<ActionResult> EditTaskInternal(MathematicalTask mathematicaltask, Subject sub,
            int isAnswerChanged, string answerType, string valueAns, string textAns, IList<string> multiChoiceList,
            IList<string> multiAnswerList, ICollection<string> singleChoiceList, int singleCorrectNo)
        {
            // First let's see if the answer for the task is being changed or not.
            // If it's not then we have an easy case to deal with.
            if (isAnswerChanged == 0)
            {
                UpdateSubjectTime(sub);

                mathematicaltask.CorrectAnswers = 0;
                mathematicaltask.Attempts = 0;

                RemoveOldImagesFromBloB(mathematicaltask.ID);
                mathematicaltask.ModifiedText = await UploadImagesToBlob(mathematicaltask.Text, mathematicaltask.ID);

                db.Entry(mathematicaltask).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Subject", new { id = mathematicaltask.SubjectID });
            }

            // Variable holding answer that belongs to the task
            var ans = db.Answers.Find(mathematicaltask.ID);

            // And now let's cope with the case when the answer is being changed as well as the task.
            switch (answerType)
            {
                case Answer.SINGLE_VALUE_ANSWER:
                    if (String.IsNullOrEmpty(valueAns))
                    {
                        // In this case we want to reload the Edit page with the original MathematicalTask
                        return ReloadPageWithStatement(db.MathematicalTasks.Find(mathematicaltask.ID), answerType, sub.Name, sub.ID);
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
                        removeOldChoices(ans.className(), mathematicaltask.ID);
                        db.Answers.Remove(ans);
                        db.SaveChanges();

                        var singleValueAnswer = new SingleValueAnswer(valueAns) { TaskID = mathematicaltask.ID };
                        db.SingleValueAnswers.Add(singleValueAnswer);
                    }

                    break;
                case Answer.TEXT_ANSWER:
                    if (String.IsNullOrEmpty(textAns))
                    {
                        // In this case we want to reload the Edit page with the original MathematicalTask
                        return ReloadPageWithStatement(db.MathematicalTasks.Find(mathematicaltask.ID), answerType, sub.Name, sub.ID);
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
                        removeOldChoices(ans.className(), mathematicaltask.ID);
                        db.Answers.Remove(ans);
                        db.SaveChanges();

                        var textAnswer = new TextAnswer(textAns) { TaskID = mathematicaltask.ID };
                        db.TextAnswers.Add(textAnswer);
                    }

                    break;
                case Answer.SINGLE_CHOICE_ANSWER:
                    if (singleCorrectNo >= singleChoiceList.Count || singleCorrectNo < 0)
                    {
                        throw new RetryLimitExceededException("The number of the correct Choice for SingleChoiceAnswer is beyond valid range.");
                    }

                    // Case when we haven't got any valid Choices for the SingleChoiceAnswer or some choices are empty
                    if (singleChoiceList.Count == 0 || singleChoiceList.Count(String.IsNullOrEmpty) != 0)
                    {
                        // In this case we want to reload the Edit page with the original MathematicalTask
                        return ReloadPageWithStatement(db.MathematicalTasks.Find(mathematicaltask.ID), answerType, sub.Name, sub.ID);
                    }

                    if (ans.className() == answerType)
                    {
                        // We don't have to create a completely new Answer, only update the current one
                        ((SingleChoiceAnswer)ans).CorrectAnswer = singleCorrectNo;
                        db.Entry(ans).State = EntityState.Modified;

                        // Let's remove old choices ...
                        removeOldChoices(ans.className(), mathematicaltask.ID);

                        // ... and add new ones
                        foreach (var singleChoice in singleChoiceList.Select(s => new SingleChoice(s) { SingleChoiceAnswerID = mathematicaltask.ID }))
                        {
                            singleChoice.SingleChoiceAnswerID = mathematicaltask.ID;
                            db.SingleChoices.Add(singleChoice);
                        }
                    }
                    else
                    {
                        // Delete previous answer and create new SingleChoiceAnswer
                        removeOldChoices(ans.className(), mathematicaltask.ID);
                        db.Answers.Remove(ans);
                        db.SaveChanges();

                        var singleChoiceAnswer = new SingleChoiceAnswer(singleCorrectNo) { TaskID = mathematicaltask.ID };
                        db.SingleChoiceAnswers.Add(singleChoiceAnswer);

                        foreach (var singleChoice in singleChoiceList.Select(s => new SingleChoice(s) { SingleChoiceAnswerID = mathematicaltask.ID }))
                        {
                            db.SingleChoices.Add(singleChoice);
                        }
                    }

                    break;
                case Answer.MULTIPLE_CHOICE_ANSWER:
                    if (multiChoiceList.Count != multiAnswerList.Count)
                    {
                        throw new RetryLimitExceededException("The number of Options doesn't equal the number of true-false answers.");
                    }

                    // Case when we haven't got any valid Choices for the MultiChoiceAnswer
                    if (multiChoiceList.Count(t => !String.IsNullOrEmpty(t)) == 0)
                    {
                        // In this case we want to reload the Edit page with the original MathematicalTask
                        return ReloadPageWithStatement(db.MathematicalTasks.Find(mathematicaltask.ID), answerType, sub.Name, sub.ID);
                    }

                    if (ans.className() == answerType)
                    {
                        // We don't have to create a completely new Answer, only update the current one
                        // Let's remove old choices ...
                        removeOldChoices(ans.className(), mathematicaltask.ID);

                        // ... and add new ones
                        for (var i = 0; i < multiChoiceList.Count; i++)
                        {
                            // No safety check whether the values send by POST are valid ones ('True' and 'False' are valid).
                            // Every invalid value will be interpreted as 'False'
                            var tempAns = (multiAnswerList[i].Equals("True", StringComparison.OrdinalIgnoreCase));
                            var multiChoice = new MultiChoice(multiChoiceList[i], tempAns) { MultipleChoiceAnswerID = mathematicaltask.ID };
                            db.MultiChoices.Add(multiChoice);
                        }
                    }
                    else
                    {
                        // Delete previous answer and create new MultipleChoiceAnswer
                        removeOldChoices(ans.className(), mathematicaltask.ID);
                        db.Answers.Remove(ans);
                        db.SaveChanges();

                        var multiChoiceAnswer = new MultipleChoiceAnswer { TaskID = mathematicaltask.ID };
                        db.MultipleChoiceAnswers.Add(multiChoiceAnswer);

                        for (var i = 0; i < multiChoiceList.Count; i++)
                        {
                            // No safety check whether the values send by POST are valid ones ('True' and 'False' are valid).
                            // Every invalid value will be interpreted as 'False'
                            var tempAns = (multiAnswerList[i].Equals("True", StringComparison.OrdinalIgnoreCase));
                            var multiChoice = new MultiChoice(multiChoiceList[i], tempAns) { MultipleChoiceAnswerID = mathematicaltask.ID };
                            db.MultiChoices.Add(multiChoice);
                        }
                    }

                    break;
            }

            UpdateSubjectTime(sub);

            mathematicaltask.CorrectAnswers = 0;
            mathematicaltask.Attempts = 0;

            RemoveOldImagesFromBloB(mathematicaltask.ID);
            mathematicaltask.ModifiedText = await UploadImagesToBlob(mathematicaltask.Text, mathematicaltask.ID);                

            db.Entry(mathematicaltask).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Details", "Subject", new { id = mathematicaltask.SubjectID });
        }

        // Private function that checks whether the value of answerType argument
        // is one of the following: SINGLE_CHOICE_ANSWER or MULTIPLE_CHOICE_ANSWER.
        // If it is, the function removes from database all choices associated with
        // Answer instance with ID attribute == id.
        private void removeOldChoices(string answerType, int id)
        {
            switch (answerType)
            {
                case Answer.SINGLE_CHOICE_ANSWER:
                    var previousSingleChoices =
                        db.SingleChoices.Where(s => s.SingleChoiceAnswerID == id).ToArray();
                    for (var j = previousSingleChoices.Length - 1; j >= 0; --j)
                    {
                        db.SingleChoices.Remove(previousSingleChoices[j]);
                    }

                    break;
                case Answer.MULTIPLE_CHOICE_ANSWER:
                    var previousMultiChoices =
                        db.MultiChoices.Where(s => s.MultipleChoiceAnswerID == id).ToArray();
                    for (var j = previousMultiChoices.Length - 1; j >= 0; --j)
                    {
                        db.MultiChoices.Remove(previousMultiChoices[j]);
                    }

                    break;
            }
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
                // Let's delete the Answer, then images from BLOB Storage and the MathematicalTask itself.
                DeleteMathematicalTasksAnswer(id);
                RemoveOldImagesFromBloB(id);
                db.MathematicalTasks.Remove(mathematicaltask);

                UpdateSubjectTime(db.Subjects.Find(subjectId));

                db.SaveChanges();
            }
            catch (RetryLimitExceededException /* dex */)
            {
                // Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            return RedirectToAction("Details", "Subject", new { id = subjectId });
        }

        // Private function that gets an id of a RegularTask and deletes the Answer
        // associated with that task. 
        private void DeleteMathematicalTasksAnswer(int id)
        {
            var rt = db.MathematicalTasks.Find(id);

            // If the Answer is either SingleChoiceAnswer or MultipleChoiceAnswer we need to
            // take care of choices linked with that Answer.
            removeOldChoices(rt.Answer.className(), id);

            db.Answers.Remove(rt.Answer);
        }

        // Private helper function, that updates given Subject's LastUpdatedTime attribute
        // and changes its database state to 'Modified'.
        private void UpdateSubjectTime(Subject s)
        {
            s.UpdateLastUpdatedTime();
            db.Entry(s).State = EntityState.Modified;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private static async Task<Stream> LoadEquationImage(string exp, bool isInline)
        {
            var requestUri = isInline
                ? "http://latex.codecogs.com/png.download?%5Cinline%20" + exp
                : "http://latex.codecogs.com/png.download?%20" + exp;
            using (var client = new HttpClient())
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
                var responseMessage = await client.SendAsync(requestMessage);
                return await responseMessage.Content.ReadAsStreamAsync();
            }
        }

        private void UploadImages(IReadOnlyList<Stream> imgStreams, IReadOnlyList<string> fileNames)
        {
            for (var i = 0; i < imgStreams.Count; i++)
            {
                var blobContainer = _blobStorageService.GetCloudBLobContainer();
                var blob = blobContainer.GetBlockBlobReference(fileNames[i]);
                blob.UploadFromStream(imgStreams[i]);
            }
        }

        public async Task<string> UploadImagesToBlob(string text, int taskId)
        {
            var regex = new Regex("(\\$[^\\$]+?\\$)|(\\$\\$[^\\$]+?\\$\\$)");
            var start = 0;
            var newText = "";
            var i = 0;
            var imgStreams = new List<Stream>();
            var imgNames = new List<string>();
            foreach (Match match in regex.Matches(text))
            {
                //System.Diagnostics.Debug.WriteLine("Value: " + match.Value);
                var matchVal = match.Value;
                var notInline = matchVal.StartsWith("$$");
                var imgStream = notInline
                    ? await LoadEquationImage(match.Value.Substring(2, match.Value.Length - 4), false)
                    : await LoadEquationImage(match.Value.Substring(1, match.Value.Length - 2), true);
                imgStreams.Add(imgStream);
                imgNames.Add("task_" + taskId + "_img_" + i.ToString(CultureInfo.InvariantCulture));
                newText += text.Substring(start, match.Index - start);

                newText += notInline
                    ? "$$[task_" + taskId + "_img_" + i.ToString(CultureInfo.InvariantCulture) + "]$$"
                    : "$[task_" + taskId + "_img_" + i.ToString(CultureInfo.InvariantCulture) + "]$";
                start = match.Index + match.Length;
                i++;
            }
            UploadImages(imgStreams, imgNames);
            newText += text.Substring(start, text.Length - start);
            //System.Diagnostics.Debug.WriteLine("New text: " + newText);
            return newText;
        }

        private const string BLOB_URL_PATH =
            "https://clarifierblob.blob.core.windows.net/clarifiermathimages/task_";

        private void RemoveOldImagesFromBloB(int taskId)
        {
            var str = BLOB_URL_PATH + taskId + "_img_";

            var blobContainer = _blobStorageService.GetCloudBLobContainer();
            var blobs = blobContainer.ListBlobs().Where(blobItem => blobItem.Uri.ToString().StartsWith(str)).
                Select(blobItem => blobItem.Uri.ToString()).ToList();

            var tempBlobStrings = blobContainer.ListBlobs().Select(blobItem => blobItem.Uri.ToString()).ToList();

            foreach (var blob in from blobStr in blobs select new Uri(blobStr) into uri select Path.GetFileName(uri.LocalPath) into fileName select blobContainer.GetBlockBlobReference(fileName))
            {
                blob.Delete();
            }
        }
    }
}
