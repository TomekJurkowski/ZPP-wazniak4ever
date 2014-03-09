using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using WazniakWebsite.DAL;

namespace WazniakWebsite.Models
{
    public class SingleChoiceAnswer : Answer
    {
        [DisplayName("Correct Answer")]
        [Required(ErrorMessage = "You have to specify the correct answer.")]
        public int CorrectAnswer { get; set; }

        public virtual ICollection<SingleChoice> SingleChoices { get; set; }

        public SingleChoiceAnswer(int correctAnswer)
        {
            CorrectAnswer = correctAnswer;
        }

        public SingleChoiceAnswer()
        {
            
        }

        public override string Overview()
        {
            return "Single Choice Answer with the " + CorrectAnswer + " option correct.";
        }

        public override string ToString()
        {
            var db = new SchoolContext();
            var mySingleChoices = db.SingleChoices.Where(s => s.SingleChoiceAnswerID == TaskID);

            var builder = new StringBuilder();

            builder.Append("There are ").Append(mySingleChoices.Count()).Append(" possibilities to choose from:");
            var i = 0;
            foreach (var singleChoice in mySingleChoices)
            {
                builder.Append(" [ ").Append(singleChoice).Append(i == CorrectAnswer ? " - CORRECT OPTION ];" : " ];");
                i++;
            }

            return builder.ToString();
        }

        public override string className()
        {
            return SINGLE_CHOICE_ANSWER;
        }
    }
}