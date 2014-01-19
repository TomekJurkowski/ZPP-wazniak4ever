using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public enum ExerciseType { MultipleChoice, Open }

    public abstract class AnyExercise
    {
        public string Question { get; set; }
        public ExerciseType Type { get; set; }

        public abstract bool[] Check(Answer a);

    }

    public class MultipleChoiceExercise : AnyExercise
    {
        public List<string> Choices { get; set; }
        public List<MultipleChoiceSolution> Solutions { get; set; }

        public MultipleChoiceExercise()
        {
            this.Type = ExerciseType.MultipleChoice;
        }

        public override bool[] Check(Answer a)
        {
            bool[] feedback = new bool[Solutions.Count];
            for (int i = 0; i < Solutions.Count; i++)
            {
                feedback[i] = (Solutions[i].IsTrue && a.Choices[i]) || (!Solutions[i].IsTrue && !a.Choices[i]);
            }

            return feedback;
        }
    }

    public class OpenExercise : AnyExercise
    {
        public OpenExerciseSolution Solution { get; set; }

        public OpenExercise()
        {
            this.Type = ExerciseType.Open;
        }

        public override bool[] Check(Answer a)
        {
            bool[] feedback = new bool[1];
            if (a.Text.CompareTo(Solution.Answer) == 0)
            {
                feedback[0] = true;
            }
            return feedback;
        }
    }

    public abstract class Solution {
        public string Explanation { get; set; }
    }

    public class OpenExerciseSolution : Solution
    {
        public string Answer { get; set; }

        public OpenExerciseSolution(string answer, string explanation)
        {
            this.Answer = answer;
            this.Explanation = explanation;
        }
    }

    public class MultipleChoiceSolution : Solution
    {
        public bool IsTrue { get; set; }

        public MultipleChoiceSolution(bool isTrue, string explanation)
        {
            this.IsTrue = IsTrue;
            this.Explanation = explanation;
        }
                
    }

    public class Answer
    {
        public bool[] Choices { get; set; }
        public string Text { get; set; }

    }

}
