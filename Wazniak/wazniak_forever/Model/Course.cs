using Microsoft.WindowsAzure.MobileServices;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Controls;
using System.IO;
using System.Threading.Tasks;
using SQLite;
using Newtonsoft.Json;

namespace wazniak_forever.Model
{
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
        public IMobileServiceTable<UserFullSubject> MySubjects = MobileService.GetTable<UserFullSubject>();
        public IMobileServiceTable<MultipleChoiceExerciseOption> MultipleChoiceOptions = 
            MobileService.GetTable<MultipleChoiceExerciseOption>();
        public IMobileServiceTable<SingleChoiceExerciseOption> SingleChoiceOptions =
            MobileService.GetTable<SingleChoiceExerciseOption>();


        public SQLiteAsyncConnection Connect = new SQLiteAsyncConnection(
            Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path,
            "offlineMode.db"), true);

        public async void Initialize()
        {
            await Connect.CreateTableAsync<Subject>();
            await Connect.CreateTableAsync<TaskAnswer>();
            await Connect.CreateTableAsync<MultipleChoiceExerciseOption>();
        }

        public async void Drop()
        {
            await Connect.DropTableAsync<Subject>();
            await Connect.DropTableAsync<TaskAnswer>();
            await Connect.DropTableAsync<MultipleChoiceExerciseOption>();
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

        public async Task<List<MultipleChoiceExerciseOption>> LoadMultipleChoiceExOptionsOffline(int subjectID)
        {
            return await Connect.Table<MultipleChoiceExerciseOption>()
                .Where(option => option.SubjectID == subjectID).ToListAsync();
        }

        public async System.Threading.Tasks.Task SaveSubjectLocally(Subject newSubject)
        {
            try
            {
                
                await Connect.InsertAsync(newSubject);
                var tasksWithAnswers = await TasksWithAnswers.Where(task => task.SubjectID == newSubject.ID).ToListAsync();
                var multipleChoiceExerciseOptions = await MultipleChoiceOptions.Where(option => option.SubjectID == newSubject.ID).ToListAsync();

                await Connect.InsertAllAsync(tasksWithAnswers);
                await Connect.InsertAllAsync(multipleChoiceExerciseOptions);

                App.ViewModel.ShowToast("Course successfully saved!");
            }
            catch 
            {
                System.Diagnostics.Debug.WriteLine("Subject is already in the local database.");            
            }
        }

        public async System.Threading.Tasks.Task DeleteSubjectFromDownloads(Subject subject)
        {
            
            await Connect.DeleteAsync(subject);
            var tasks = await LoadExercisesOffline(subject.ID);
            tasks.ForEach(async task => 
            { 
                await Connect.DeleteAsync(task); 
            });
            var options = await LoadMultipleChoiceExOptionsOffline(subject.ID);
            options.ForEach(async option =>
            {
                await Connect.DeleteAsync(option);
            });
            
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
                await Connect.UpdateAsync(externalSubject);
                var localTasks = await LoadExercisesOffline(subject.ID);
                localTasks.ForEach(async task =>
                {
                    await Connect.DeleteAsync(task);
                });

                var externalTasks = await TasksWithAnswers.Where(task => task.SubjectID == subject.ID).ToListAsync();
                await Connect.InsertAllAsync(externalTasks);

                var localMultipleChoiceExOptions = await LoadMultipleChoiceExOptionsOffline(subject.ID);
                localMultipleChoiceExOptions.ForEach(async option =>
                {
                    await Connect.DeleteAsync(option);
                });

                var externalMultipleChoiceExOptions = await MultipleChoiceOptions
                    .Where(option => option.SubjectID == subject.ID).ToListAsync();
                await Connect.InsertAllAsync(externalMultipleChoiceExOptions);
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
            }
        }

    }

    #region Azure Adapters

    public class SingleChoiceExerciseOption
    {
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
        public double Percentage { get; set; }
        public int Attempts { get; set; }
    }

    public class UserSubject
    {
        public string Id { get; set; }
        public string UserID { get; set; }
        public int SubjectID { get; set; }
        public double Percentage { get; set; }
        public int Attempts { get; set; }

        public UserSubject(string id, string userID, int subjectID, double percentage, int attempts)
        {
            Id = id;
            UserID = userID;
            SubjectID = subjectID;
            Percentage = percentage;
            Attempts = attempts;
        }

        public UserSubject() { }
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
    }

    public abstract class Exercise
    {
        [PrimaryKey]
        public int ID { get; set; }
        public int SubjectID { get; set; }
        public int SolutionID { get; set; }
        public string Title { get; set; }
        public Subject Subject { get; set; }
        public Solution Solution { get; set; }
    }

    public class RegularExercise : Exercise
    {
        public string Question { get; set; }

        public RegularExercise() {}

        public RegularExercise(int id, int subjectId, int solutionId, 
            string title, string question, Subject subject, Solution solution)
        {
            ID = id;
            SubjectID = subjectId;
            Title = title;
            Question = question;
            Subject = subject;
            Solution = solution;
        }
    }

    public class MathExercise : Exercise
    {
        public Image Question { get; set; }
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