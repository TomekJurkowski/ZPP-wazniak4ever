using Microsoft.WindowsAzure.MobileServices;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Controls;
using System.IO;
using System.Threading.Tasks;

namespace wazniak_forever.Model
{
    public class DatabaseContext
    {
        public static MobileServiceClient MobileService = new MobileServiceClient(
            "https://clarifier.azure-mobile.net/",
            "QDgknqkjpjxNvyJlrQvBoFjHvhXDaa88"
        );

        public IMobileServiceTable<Subject> Subjects = MobileService.GetTable<Subject>();

        /*public SQLiteAsyncConnection Connect = new SQLiteAsyncConnection(
            Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path,
            "offlineMode.db"), true);

        public async void Initialize()
        {
            await Connect.CreateTableAsync<Subject>();
        }

        public async Task<IEnumerable<Subject>> LoadSubjectsOffline()
        {
            return await Connect.Table<Subject>().ToListAsync();
        }

        public async void SaveSubjectLocally(Subject newSubject)
        {
            await Connect.InsertAsync(newSubject);
        }

        public async void SyncLocalDatabase()
        {
            var localSubjects = Connect.Table<Subject>();
            var orderedSubjects = await from lSubject in localSubjects
                               orderby lSubject.LastUpdated descending
                               select lSubject;
            var earliest = await orderedSubjects.FirstOrDefaultAsync();
            var updated =
               earliest != null ? earliest.LastUpdated : DateTime.MinValue;

            var updatedSubjects =
              await (from subject in MobileService.GetTable<Subject>()
                     where subject.LastUpdated > updated
                     select subject).ToListAsync();
            foreach (var s in updatedSubjects)
            {
                if (localSubjects.Any(x => x.ID == s.ID)) await Connect.UpdateAsync(s);
            }
        }*/

    }

    public class Subject
    {
        public int ID;
        public string Name { get; private set; }
        public string Description { get; set; }
        public Subject(string Name)
        {
            this.Name = Name;
        }
    }

    public abstract class Exercise
    {
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
        public bool Equals(SingleAnswer<T> other) { return value.CompareTo(other.value) != 0; }
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
        public int ExerciseID { get; set; }
        public List<string> Choices { get; set; }
        public AnswerType Answer { get; set; }
        public Exercise Exercise { get; set; }
    }

    public class TextSolution : Solution
    {
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
        public MultipleChoiceSolution(int _id, List<string> _choices, List<string> _answers, Exercise _ex)
        {
            ExerciseID = _id;
            Choices = _choices;
            Answer = new AnswerList<string>(_answers);
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