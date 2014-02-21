using System.Collections;
using System.Collections.Generic;
using System.Windows.Controls;

namespace wazniak_forever.Model
{
    public class Course
    {
        public string Name { get; private set; }

        public Course(string Name)
        {
            this.Name = Name;
        }
    }

    public class Exercise
    {
        public string Title { get; set; }
        public string Question { get; set; }
        public Solution Solution { get; set; }

        public Exercise(string _title, string _question, Solution _solution) 
        {
            Title = _title;
            Question = _question;
            Solution = _solution;
        }

        public Exercise(string _title, Solution _solution) 
        {
            Title = _title;
            Solution = _solution;
        }
    }

    public class MathExercise : Exercise
    {
        public override Image Question { get; set; }

        public MathExercise(string _title, Image _question, Solution _solution) : base(_title, _solution)
        {
            Question = _question;
        }
    }

    public abstract class AnswerType 
    {
    }

    public class SingleAnswer<T> : AnswerType where T : System.IComparable
    {
        public T value { get; set; }
        public SingleAnswer(T _value) { value = _value; }
        public bool operator ==(SingleAnswer<T> first, SingleAnswer<T> second) { return first.value.Equals(second.value); }
    }

    public class AnswerList<T> : AnswerType where T : System.IComparable
    {
        public List<T> value { get; set; }
        public AnswerList(List<T> _value) { value = _value; }
        public bool operator ==(AnswerList<T> first, AnswerList<T> second) 
        {
            if (first.value.Count != second.value.Count) return false;
            for (int i = 0; i < first.value.Count; i++)
            {
                if (!first.value[i].Equals(second.value[i])) return false;
            }
            return true; 
        }
    }

    public abstract class Solution
    {
        public abstract AnswerType Answer { get; set; }
    }

    public class TextSolution : Solution
    {
        public TextSolution(string _answer)
        {
            Answer = new SingleAnswer<string>(_answer);
        }
    }

    public class SingleValueSolution : Solution
    {
        public SingleValueSolution(int _answer)
        {
            Answer = new SingleAnswer<int>(_answer);
        }
    }

    public abstract class ChoiceSolution<T> : Solution
    {
        public List<T> Choices { get; set; }
    }

    public class MultipleChoiceSolution : ChoiceSolution<string>
    {
        public MultipleChoiceSolution(List<string> _choices, List<bool> _answers)
        {
            Choices = _choices;
            Answer = new AnswerList<bool>(_answers);
        }
    }

    public class SingleChoiceSolution : ChoiceSolution<string>
    {
        public SingleChoiceSolution(List<string> _choices, List<bool> _answers)
        {
            Choices = _choices;
            Answer = new AnswerList<bool>(_answers);
        }
    }
}
