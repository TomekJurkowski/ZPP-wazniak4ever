using Microsoft.WindowsAzure.MobileServices;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Controls;

namespace wazniak_forever.Model
{
    public class DatabaseContext
    {
        public IMobileServiceTable<Subject> Subjects = App.MobileService.GetTable<Subject>();
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

    public enum SolutionType { Open, Multiple, Single }

    public abstract class AnswerType
    {
        public virtual bool Equals(AnswerType other) { return true; } // UGLY
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
        public bool Equals(AnswerList<T> other)
        {
            if (value.Count != other.value.Count) return false;
            for (int i = 0; i < value.Count; i++)
            {
                if (!value[i].Equals(other.value[i])) return false;
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
            Answer.Type = SolutionType.Open;
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