using System.Collections;
using System.Collections.Generic;
using System.Windows.Controls;

namespace wazniak_forever.Model
{
    public class Subject
    {
        public string Name { get; private set; }

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

    public abstract class AnswerType
    {
        public int Compare(AnswerType other);
    }

    public class SingleAnswer<T> : AnswerType where T : System.IComparable
    {
        public T value { get; set; }
        public SingleAnswer(T _value) { value = _value; }
        public bool operator !=(SingleAnswer<T> first, SingleAnswer<T> second) { return first.value.CompareTo(second.value) != 0; }
    }

    public class AnswerList<T> : AnswerType where T : System.IComparable
    {
        public List<T> value { get; set; }
        public AnswerList(List<T> _value) { value = _value; }
        public bool operator !=(AnswerList<T> first, AnswerList<T> second)
        {
            if (first.value.Count != second.value.Count) return true;
            for (int i = 0; i < first.value.Count; i++)
            {
                if (!first.value[i].Equals(second.value[i])) return true;
            }
            return false;
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
            Exercise = _ex;
        }
    }

    #endregion
}
