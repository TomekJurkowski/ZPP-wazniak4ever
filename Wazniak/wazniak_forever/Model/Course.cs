using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.WindowsAzure.MobileServices;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Controls;
using System.IO;
using System.Threading.Tasks;
using SQLite;
using Newtonsoft.Json;
using System.Windows.Threading;
using System;
using wazniak_forever.Controls;

namespace wazniak_forever.Model
{
    public class DTimer
    {
        private DispatcherTimer timer;
        public event Action<int> HandleTick;

        private int counter;

        public DTimer() 
        { 
            timer = new DispatcherTimer(); 
        }

        public void Start(int periodInSeconds, int time)
        {
            timer.Interval = TimeSpan.FromSeconds(periodInSeconds);
            timer.Tick += timer_Task;
            counter = time;
            timer.Start();
        }

        public void Resume()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }
        private void timer_Task(object sender, EventArgs e)
        {
            counter--;
            if (HandleTick != null) HandleTick(counter);
            if (counter == 0) Stop();
        }

        public bool IsEnabled()
        {
            return timer.IsEnabled;
        }
    }

    public enum AuthenticationProviderType { Microsoft, Facebook, Google, Twitter }

    public class AuthenticationProvider
    {
        public AuthenticationProviderType Type { get; set; }
        public string Name { get; set; }

        public AuthenticationProvider(AuthenticationProviderType type, string name)
        {
            Type = type;
            Name = name;
        }
    }

    public class ImageAttachment
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int TaskId { get; set; }
        public byte[] ImageBytes { get; set; }
    }

    public class MathImage
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] ImageBytes { get; set; }
    }

    public class DatabaseContext
    {
        public MobileServiceUser User { get; set; }

        public static MobileServiceClient MobileService = new MobileServiceClient(
            "https://clarifier.azure-mobile.net/",
            "QDgknqkjpjxNvyJlrQvBoFjHvhXDaa88"
        );

        public IMobileServiceTable<Subject> Subjects = MobileService.GetTable<Subject>();
        public IMobileServiceTable<Task> Tasks = MobileService.GetTable<Task>();
        public IMobileServiceTable<Answer> Answers = MobileService.GetTable<Answer>();
        public IMobileServiceTable<TaskAnswer> TasksWithAnswers = MobileService.GetTable<TaskAnswer>();
        public IMobileServiceTable<UserSubject> UsersAndSubjects = MobileService.GetTable<UserSubject>();
        public IMobileServiceTable<UserExercise> UsersAndExercises = MobileService.GetTable<UserExercise>(); 
        public IMobileServiceTable<UserFullSubject> MySubjects = MobileService.GetTable<UserFullSubject>();
        public IMobileServiceTable<UserModule> UserModules = MobileService.GetTable<UserModule>();
        public IMobileServiceTable<MultipleChoiceExerciseOption> MultipleChoiceOptions = 
            MobileService.GetTable<MultipleChoiceExerciseOption>();
        public IMobileServiceTable<SingleChoiceExerciseOption> SingleChoiceOptions =
            MobileService.GetTable<SingleChoiceExerciseOption>();

        public IMobileServiceTable<Module> Modules = MobileService.GetTable<Module>();
        public IMobileServiceTable<TaskStatistics> ExerciseStatistics = MobileService.GetTable<TaskStatistics>();
        public IMobileServiceTable<ModuleStatistics> ModuleStatistics = MobileService.GetTable<ModuleStatistics>(); 

        public SQLiteAsyncConnection Connect = new SQLiteAsyncConnection(
            Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path,
            "offlineMode.db"), true);

        public async void Initialize()
        {
            await Connect.CreateTableAsync<Subject>();
            await Connect.CreateTableAsync<TaskAnswer>();
            await Connect.CreateTableAsync<MultipleChoiceExerciseOption>();
            await Connect.CreateTableAsync<SingleChoiceExerciseOption>();
            await Connect.CreateTableAsync<Module>();
            await Connect.CreateTableAsync<ImageAttachment>();
            await Connect.CreateTableAsync<MathImage>();
        }

        public async void Drop()
        {
            await Connect.DropTableAsync<Subject>();
            await Connect.DropTableAsync<TaskAnswer>();
            await Connect.DropTableAsync<MultipleChoiceExerciseOption>();
            await Connect.DropTableAsync<SingleChoiceExerciseOption>();
            await Connect.DropTableAsync<Module>();
            await Connect.DropTableAsync<ImageAttachment>();
            await Connect.DropTableAsync<MathImage>();
        }

        public async Task<List<Subject>> LoadSubjectsOffline()
        {
            return await Connect.Table<Subject>().ToListAsync();
        }

        public async Task<List<TaskAnswer>> LoadExercisesOffline(int subjectID)
        {
            return await Connect.Table<TaskAnswer>()
                .Where(exercise => exercise.SubjectID == subjectID).ToListAsync();
        }


        public async Task<List<T>> LoadExerciseChoicesOffline<T>(int subjectID) where T : SingleChoiceExerciseOption, new()
        {
            return await Connect.Table<T>()
                .Where(option => option.SubjectID == subjectID).ToListAsync();
        }

        public async Task<List<Module>> LoadModulesOffline(int subjectId)
        {
            return await Connect.Table<Module>().Where(module => module.SubjectID == subjectId).ToListAsync();
        }

        public async Task<byte[]> LoadImageAttachmentOffline(int taskId)
        {
            var image = await Connect.Table<ImageAttachment>().Where(img => img.TaskId == taskId).FirstOrDefaultAsync();
            return image != null ? image.ImageBytes : null;
        }

        private async Task<ImageAttachment> LoadImageAttachmentObjectOffline(int taskId)
        {
            var image = await Connect.Table<ImageAttachment>().Where(img => img.TaskId == taskId).FirstOrDefaultAsync();
            return image;
        }

        public async Task<byte[]> LoadMathImageOfflineByName(string name)
        {
            var image = await Connect.Table<MathImage>().Where(img => img.Name == name).FirstOrDefaultAsync();
            return image != null ? image.ImageBytes : null;
        }

        /*public async Task<List<MultipleChoiceExerciseOption>> LoadMultipleChoiceExOptionsOffline(int subjectID)
        {
            return await Connect.Table<MultipleChoiceExerciseOption>()
                .Where(option => option.SubjectID == subjectID).ToListAsync();
        }

        public async Task<List<SingleChoiceExerciseOption>> LoadSingleChoiceExOptionsOffline(int subjectID) 
        {
            return await Connect.Table<SingleChoiceExerciseOption>()
                .Where(option => option.SubjectID == subjectID).ToListAsync();
        }*/

        private async Task<List<ImageAttachment>> GetAllImageAttachments(IEnumerable<TaskAnswer> tasksWithAnswers)
        {
            var images = new List<ImageAttachment>();
            foreach (var task in tasksWithAnswers.Where(task => !string.IsNullOrEmpty(task.ImageUrl)))
            {
                var imageBytes = await RichTextBoxExtensions.LoadImageFromUrl(task.ImageUrl);
                images.Add(new ImageAttachment
                {
                    TaskId = task.ID,
                    ImageBytes = imageBytes
                });
                System.Diagnostics.Debug.WriteLine(images.Count);
            }

            return images;
        }


        private async Task<List<MathImage>> GetAllMathImagesForTask(string text)
        {
            const string BLOB_URL = "http://clarifierblob.blob.core.windows.net/clarifiermathimages/";
            var regex = new Regex("(\\$[^\\$]+?\\$)|(\\$\\$[^\\$]+?\\$\\$)");
            var images = new List<MathImage>();
            foreach (Match match in regex.Matches(text))
            {
                var matchVal = match.Value;
                var notInline = matchVal.StartsWith("$$");
                var imageId = notInline
                    ? match.Value.Substring(3, match.Value.Length - 6)
                    : match.Value.Substring(2, match.Value.Length - 4);
                var imageUrl = BLOB_URL + imageId;
                var imageBytes = await RichTextBoxExtensions.LoadImageFromUrl(imageUrl);
                images.Add(new MathImage
                {
                    Name = imageId,
                    ImageBytes = imageBytes
                });
            }
            return images;
        }

        private async Task<List<MathImage>>  GetAllMathImages(IEnumerable<TaskAnswer> tasksWithAnswers)
        {
            var images = new List<MathImage>();

            foreach (var task in tasksWithAnswers.Where(task => task.TaskDiscriminator == "MathematicalTask"))
            {
                var taskImages = await GetAllMathImagesForTask(task.ModifiedText);
                images.AddRange(taskImages);
            }

            return images;
        }


        private async System.Threading.Tasks.Task InsertIntoLocalDatabase(Subject newSubject)
        {
            await Connect.InsertAsync(newSubject);
            var tasksWithAnswers = await TasksWithAnswers.Where(task => task.SubjectID == newSubject.ID).ToListAsync();
            var multipleChoiceExerciseOptions = await MultipleChoiceOptions.Where(option => option.SubjectID == newSubject.ID).ToListAsync();
            var singleChoiceExerciseOptions = await SingleChoiceOptions.Where(option => option.SubjectID == newSubject.ID).ToListAsync();
            var modules = await Modules.Where(module => module.SubjectID == newSubject.ID).ToListAsync();

            var images = await GetAllImageAttachments(tasksWithAnswers);
            var mathImages = await GetAllMathImages(tasksWithAnswers);            

            await Connect.InsertAllAsync(tasksWithAnswers);
            await Connect.InsertAllAsync(multipleChoiceExerciseOptions);
            await Connect.InsertAllAsync(singleChoiceExerciseOptions);
            await Connect.InsertAllAsync(modules);
            await Connect.InsertAllAsync(images);
            await Connect.InsertAllAsync(mathImages);
        }

        public async System.Threading.Tasks.Task SaveSubjectLocally(Subject newSubject)
        {
            try
            {
                await InsertIntoLocalDatabase(newSubject);

                App.ViewModel.ShowToast("Course successfully saved!");
            }
            catch 
            {
                System.Diagnostics.Debug.WriteLine("Subject is already in the local database.");            
            }
        }



        private async System.Threading.Tasks.Task DeleteFromLocalDatabase(Subject subject)
        {
            await Connect.DeleteAsync(subject);
            var tasks = await LoadExercisesOffline(subject.ID);
            tasks.ForEach(async task =>
            {
                await Connect.DeleteAsync(task);
                var imageAttachment = await LoadImageAttachmentObjectOffline(task.ID);
                if (imageAttachment != null)
                    await Connect.DeleteAsync(imageAttachment);
                if (task.TaskDiscriminator == "MathematicalTask")
                {
                    var mathImages = await GetAllMathImagesForTask(task.ModifiedText);
                    mathImages.ForEach(async image =>
                    {
                        await Connect.DeleteAsync(image);
                    });
                }              
            });
            var multipleChoiceOptions = await LoadExerciseChoicesOffline<MultipleChoiceExerciseOption>(subject.ID);
            multipleChoiceOptions.ForEach(async option =>
            {
                await Connect.DeleteAsync(option);
            });
            var singleChoiceOptions = await LoadExerciseChoicesOffline<SingleChoiceExerciseOption>(subject.ID);
            singleChoiceOptions.ForEach(async option =>
            {
                await Connect.DeleteAsync(option);
            });
            var modules = await LoadModulesOffline(subject.ID);
            modules.ForEach(async module =>
            {
                await Connect.DeleteAsync(module);
            });
        }

        public async System.Threading.Tasks.Task DeleteSubjectFromDownloads(Subject subject)
        {
            await DeleteFromLocalDatabase(subject);
            
            App.ViewModel.ShowToast("Course successfully deleted!");
            await App.ViewModel.LoadDownloadedCourses();
        }

        public async Task<bool> CheckIfSubjectSavedLocally(Subject subject)
        {
            var result = await Connect.Table<Subject>().Where(x => x.ID == subject.ID).ToListAsync();
            return (result.Count > 0);
        }

        public async void SyncLocalDatabase()
        {
            var localSubjects = Connect.Table<Subject>();
            var orderedSubjects = from lSubject in localSubjects
                               orderby lSubject.LastUpdated descending
                               select lSubject;
            var earliest = await orderedSubjects.FirstOrDefaultAsync();
            var updated =
               earliest != null ? earliest.LastUpdated : System.DateTime.MinValue;

            var updatedSubjects =
              await (from subject in MobileService.GetTable<Subject>()
                     where subject.LastUpdated > updated
                     select subject).ToListAsync();
            foreach (var s in updatedSubjects)
            {
                if ((await localSubjects.ToListAsync()).Find(x => x.ID == s.ID) != null) await Connect.UpdateAsync(s);
            }
        }

        public async System.Threading.Tasks.Task SyncDownloadedCourse(Subject subject)
        {
            var localSubject = await Connect.Table<Subject>().Where(s => s.ID == subject.ID).FirstOrDefaultAsync();
            if (localSubject == null) return;
            var externalSubject = (await MobileService.GetTable<Subject>().Where(s => s.ID == subject.ID).ToListAsync())[0];
            if (externalSubject.LastUpdated > localSubject.LastUpdated)
            {
                await DeleteFromLocalDatabase(externalSubject);
                await InsertIntoLocalDatabase(externalSubject);

                App.ViewModel.ShowToast("The course has been updated!");

                #region Temporary Code
                /*await Connect.UpdateAsync(externalSubject);

                var localTasks = await LoadExercisesOffline(subject.ID);
                localTasks.ForEach(async task =>
                {
                    await Connect.DeleteAsync(task);
                });

                var externalTasks = await TasksWithAnswers.Where(task => task.SubjectID == subject.ID).ToListAsync();
                await Connect.InsertAllAsync(externalTasks);

                var localMultipleChoiceExOptions = await LoadExerciseChoicesOffline<MultipleChoiceExerciseOption>(subject.ID);
                localMultipleChoiceExOptions.ForEach(async option =>
                {
                    await Connect.DeleteAsync(option);
                });

                var localSingleChoiceExOptions = await LoadExerciseChoicesOffline<SingleChoiceExerciseOption>(subject.ID);
                localSingleChoiceExOptions.ForEach(async option =>
                {
                    await Connect.DeleteAsync(option);
                });

                var externalMultipleChoiceExOptions = await MultipleChoiceOptions
                    .Where(option => option.SubjectID == subject.ID).ToListAsync();
                await Connect.InsertAllAsync(externalMultipleChoiceExOptions);

                var externalSingleChoiceExOptions = await SingleChoiceOptions
                    .Where(option => option.SubjectID == subject.ID).ToListAsync();
                await Connect.InsertAllAsync(externalSingleChoiceExOptions);*/

                /*var localTasks = await LoadExercisesOffline(subject.ID);
                var externalTasks = await TasksWithAnswers.Where(task => task.SubjectID == subject.ID).ToListAsync();
                int i = 0;
                int j = 0;
                while (j < externalTasks.Count) 
                {
                    if (i >= localTasks.Count || localTasks[i].ID > externalTasks[j].ID) {
                        await Connect.InsertAsync(externalTasks[j]);
                        j++;
                        continue;
                    }
                    if (localTasks[i].ID == externalTasks[j].ID)
                    {
                        await Connect.UpdateAsync(externalTasks[j]);
                        i++;
                        j++;
                    }
                    else if (localTasks[i].ID < externalTasks[j].ID)
                    {
                        while (i < localTasks.Count && localTasks[i].ID < externalTasks[j].ID)
                        {
                            await Connect.DeleteAsync(localTasks[i]);
                            i++;
                        }
                    }
                }*/
                #endregion
            }
            else
            {
                App.ViewModel.ShowToast("The course is up to date!");
            }
        }

    }

    #region Azure Adapters

    public class SingleChoiceExerciseOption
    {
        [PrimaryKey]
        public int ID { get; set; }
        public int SubjectID { get; set; }
        public int TaskID { get; set; }
        public string ChoiceString { get; set; }
    }

    public class MultipleChoiceExerciseOption : SingleChoiceExerciseOption
    {
        public bool ChoiceBool { get; set; }
    }

    public class UserFullSubject
    {
        public string UserID { get; set; }
        public int ID { get; set; }
        public string MappingID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public System.DateTime LastUpdated { get; set; }
        public int CurrentModuleIndex { get; set; }
        public int CorrectAnswers { get; set; }
        public int Attempts { get; set; }
        public System.DateTime LastAttempt { get; set; }
    }

    public class UserSubject
    {
        public string ID { get; set; }
        public string UserID { get; set; }
        public int SubjectID { get; set; }
        public int CurrentModuleIndex { get; set; }
        public int CorrectAnswers { get; set; }
        public int Attempts { get; set; }
        public System.DateTime LastAttempt { get; set; }

        public UserSubject(string id, string userID, int subjectID, int currentModuleIndex, int correctAnswers, int attempts, System.DateTime lastAttempt) :
            this(userID, subjectID, currentModuleIndex, correctAnswers, attempts, lastAttempt)
        {
            ID = id;
        }

        public UserSubject(string userID, int subjectID, int currentModuleIndex, int correctAnswers, int attempts, System.DateTime lastAttempt)
        {
            UserID = userID;
            SubjectID = subjectID;
            CurrentModuleIndex = currentModuleIndex;
            CorrectAnswers = correctAnswers;
            Attempts = attempts;
            LastAttempt = lastAttempt;
        }

        public UserSubject() { }
    }

    public class UserModule
    {
        public string ID { get; set; }
        public string UserID { get; set; }
        public int ModuleID { get; set; }
        public int SubjectID { get; set; }
        public int SequenceNo { get; set; }
        public int AnswersNumber { get; set; }
        public const int ATTEMPTS = 10;
        public const int DONE = 8;

        public bool ans1 { get; set; }
        public bool ans2 { get; set; }
        public bool ans3 { get; set; }
        public bool ans4 { get; set; }
        public bool ans5 { get; set; }
        public bool ans6 { get; set; }
        public bool ans7 { get; set; }
        public bool ans8 { get; set; }
        public bool ans9 { get; set; }
        public bool ans10 { get; set; }

        public UserModule() { }
        
        public UserModule(string id, string userID, int moduleID, int subjectID, int sequenceNo, int answersNumber, List<bool> answers) : this(userID, moduleID, subjectID, sequenceNo, answersNumber, answers)
        {
            ID = id;
        }

        public UserModule(string userID, int moduleID, int subjectID, int sequenceNo, int answersNumber, List<bool> answers)
        {
            UserID = userID;
            ModuleID = moduleID;
            SubjectID = subjectID;
            SequenceNo = sequenceNo;
            AnswersNumber = answersNumber;
            parseAnswers(answers);
        }

        public void parseAnswers(List<bool> answers) 
        {
            if (answers.Count > 0) ans1 = answers[0];
            if (answers.Count > 1) ans2 = answers[1];
            if (answers.Count > 2) ans3 = answers[2];
            if (answers.Count > 3) ans4 = answers[3];
            if (answers.Count > 4) ans5 = answers[4];
            if (answers.Count > 5) ans6 = answers[5];
            if (answers.Count > 6) ans7 = answers[6];
            if (answers.Count > 7) ans8 = answers[7];
            if (answers.Count > 8) ans9 = answers[8];
            if (answers.Count > 9) ans10 = answers[9];
            AnswersNumber = answers.Count;
        }

        public List<bool> parseAnswersToList(int space)
        {
            List<bool> result = new List<bool>();
            if (AnswersNumber > 0) result.Add(ans1);
            if (AnswersNumber > 1) result.Add(ans2);
            if (AnswersNumber > 2) result.Add(ans3);
            if (AnswersNumber > 3) result.Add(ans4);
            if (AnswersNumber > 4) result.Add(ans5);
            if (AnswersNumber > 5) result.Add(ans6);
            if (AnswersNumber > 6) result.Add(ans7);
            if (AnswersNumber > 7) result.Add(ans8);
            if (AnswersNumber > 8) result.Add(ans9);
            if (AnswersNumber > 9) result.Add(ans10);
            if (space < AnswersNumber) result.RemoveRange(0, AnswersNumber - space);
            return result;
        }

        public int CountCorrectAnswers()
        {
            return parseAnswersToList(AnswersNumber).FindAll(b => b == true).Count;
        }

        public void AddAnswers(List<bool> answers)
        {
            List<bool> resultList = new List<bool>();
            if (answers.Count < UserModule.ATTEMPTS)
            {
                resultList = parseAnswersToList(UserModule.ATTEMPTS - answers.Count);
                resultList.AddRange(answers);
            }
            else resultList = answers;
            parseAnswers(resultList);
        }

        public bool CheckAnswers()
        {
            return (CountCorrectAnswers() >= DONE);
        }
    }

    public class UserExercise
    {
        public string ID { get; set; }
        public string UserID { get; set; }
        public int SubjectID { get; set; }
        public int ExerciseID { get; set; }
        public int Attempts { get; set; }
        public int CorrectAnswers { get; set; }
        public System.DateTime LastAttempt { get; set; }

        public UserExercise() { }

        public UserExercise(string userId, int subjectId, int exerciseId, int attempts, int correctAnswers, System.DateTime lastAttempt)
        {
            UserID = userId;
            SubjectID = subjectId;
            ExerciseID = exerciseId;
            Attempts = attempts;
            CorrectAnswers = correctAnswers;
            LastAttempt = lastAttempt;
        }
    }

    public class TaskAnswer
    {
        [PrimaryKey]
        public int ID { get; set; }
        public string Title { get; set; }
        public int SubjectID { get; set; }
        public string TaskText { get; set; }
        public string Text1 { get; set; }
        public string TaskDiscriminator { get; set; }
        public int TaskID { get; set; }
        public string Value { get; set; }
        public string AnswerText { get; set; }
        public int CorrectAnswer { get; set; }
        public string AnswerDiscriminator { get; set; }
        public int ModuleID { get; set; }
        public string ModifiedText { get; set; }
        public string ImageUrl { get; set; }
    }

    public class Task
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public int SubjectID { get; set; }
        public Subject Subject { get; set; }
        public Answer Answer { get; set; }
        public string Text { get; set; }
        public string Text1 { get; set; }
        public string Discriminator { get; set; }
    }

    public class Answer
    {
        public int TaskID { get; set; }
        public virtual Task Task { get; set; }
        public string Value { get; set; }
        public string Text { get; set; }
        public string Discriminator { get; set; }
    }

    #endregion

    public enum CourseType { Classic, StudyWithClarifier, Time, FixedNumber }

    public class Subject
    {
        [PrimaryKey]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public System.DateTime LastUpdated { get; set; }
        public Subject() { }

        public Subject(string Name)
        {
            this.Name = Name;
        }

        public Subject(int id, string name, 
            string description, System.DateTime lastUpdated)
        {
            ID = id;
            Name = name;
            Description = description;
            LastUpdated = lastUpdated;
        }

        public void UpdateLastUpdatedTime()
        {
            LastUpdated = DateTime.Now;
        }
    }

    public class Module
    {
        [PrimaryKey]
        public int ID { get; set; }
        public int SubjectID { get; set; }
        public int SequenceNo { get; set; }
        public string Title { get; set; }

        public Module(string name)
        {
            this.Title = name;
        }

        public Module()
        {
        }
    }

    public abstract class Exercise
    {
        [PrimaryKey]
        public int ID { get; set; }
        public int SubjectID { get; set; }
        public int ModuleID { get; set; }
        public int SolutionID { get; set; }
        public string Title { get; set; }
        public Subject Subject { get; set; }
        public Solution Solution { get; set; }
        public string ModifiedText { get; set; }
        public string ImageUrl { get; set; }
    }

    public class RegularExercise : Exercise
    {
        public string Question { get; set; }

        public RegularExercise() {}

        public RegularExercise(int id, int subjectId, int moduleId, int solutionId, 
            string title, string question, Subject subject, Solution solution, string mathText,
            string imageUrl)
        {
            ID = id;
            SubjectID = subjectId;
            ModuleID = moduleId;
            Title = title;
            Question = question;
            Subject = subject;
            Solution = solution;
            ModifiedText = mathText;
            ImageUrl = imageUrl;
        }
    }

    public class MathExercise : Exercise
    {
        public Image Question { get; set; }
    }

    public class TaskStatistics
    {
        public int ID { get; set; }
        public int TaskID { get; set; }
        public long CorrectAnswers { get; set; }
        public long Attempts { get; set; }
    }

    public class ModuleStatistics
    {
        public int ID { get; set; }
        public int ModuleID { get; set; }
        public long CorrectAnswers { get; set; }
        public long Attempts { get; set; }
    }


    #region Solutions

    public enum SolutionType { Open, Value, Multiple, Single }

    public abstract class AnswerType
    {
        public SolutionType Type { get; set; }
    }

    public class SingleAnswer<T> : AnswerType where T : System.IComparable
    {
        public T value { get; set; }
        public SingleAnswer(T _value) { value = _value; }
        public bool Equals(SingleAnswer<T> other) { return value.CompareTo(other.value) == 0; }
    }

    public class AnswerList<T> : AnswerType where T : System.IComparable
    {
        public List<T> value { get; set; }
        public AnswerList(List<T> _value) { value = _value; }
        public bool[] GetFeedback(AnswerList<T> other)
        {
            if (other == null || value.Count <= 0) return null;
            bool[] result = new bool[value.Count];
            if (value.Count != other.value.Count)
            {
                result[0] = false;
                return result;
            }
            for (int i = 0; i < value.Count; i++)
            {
                if (value[i].Equals(other.value[i])) result[i] = true;
                else result[i] = false;
            }
            return result;
        }

        public bool Equals(AnswerList<T> other)
        {
            foreach (bool b in GetFeedback(other))
            {
                if (!b) return false;
            }
            return true;
        }
    }

    public abstract class Solution
    {
        [PrimaryKey]
        public int ExerciseID { get; set; }
        public List<string> Choices { get; set; }
        public AnswerType Answer { get; set; }
        public Exercise Exercise { get; set; }
    }

    public class TextSolution : Solution
    {
        public TextSolution() { }
        public TextSolution(int _id, string _answer, Exercise _ex)
        {
            ExerciseID = _id;
            Choices = null;
            Answer = new SingleAnswer<string>(_answer);
            Answer.Type = SolutionType.Open;
            Exercise = _ex;
        }
    }

    public class SingleValueSolution : Solution
    {
        public SingleValueSolution() { }
        public SingleValueSolution(int _id, string _answer, Exercise _ex)
        {
            ExerciseID = _id;
            Choices = null;
            Answer = new SingleAnswer<string>(_answer);
            Answer.Type = SolutionType.Value;
            Exercise = _ex;
        }
    }

    public class MultipleChoiceSolution : Solution
    {
        public MultipleChoiceSolution(int _id, List<string> _choices, List<bool> _answers, Exercise _ex)
        {
            ExerciseID = _id;
            Choices = _choices;
            Answer = new AnswerList<bool>(_answers);
            Answer.Type = SolutionType.Multiple;
            Exercise = _ex;
        }
    }

    public class SingleChoiceSolution : Solution
    {
        public SingleChoiceSolution(int _id, List<string> _choices, string _answer, Exercise _ex)
        {
            ExerciseID = _id;
            Choices = _choices;
            Answer = new SingleAnswer<string>(_answer);
            Answer.Type = SolutionType.Single;
            Exercise = _ex;
        }
    }

    #endregion
}