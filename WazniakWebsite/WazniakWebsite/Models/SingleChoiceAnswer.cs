using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WazniakWebsite.Models
{
    public class SingleChoiceAnswer : Answer
    {
        [DisplayName("Correct Answer")]
        [Required(ErrorMessage = "You have to specify the correct answer.")]
        public int CorrectAnswer { get; set; }

        public virtual ICollection<SingleChoice> SingleChoices { get; set; }

        public override string Overview()
        {
            return "SingleChoiceAnswer with the " + CorrectAnswer + " option correct.";
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append("\tPossible Answers").AppendLine().AppendLine();
            var i = 0;
            foreach (var singleChoice in SingleChoices)
            {
                if (i == CorrectAnswer)
                {
                    builder.Append(++i).Append(")\t").Append(singleChoice).Append("\tCorrect Answer").AppendLine();
                }
                else
                {
                    builder.Append(++i).Append(")\t").Append(singleChoice).AppendLine();                    
                }
            }

            return builder.ToString();
        }

        public override string className()
        {
            return SINGLE_CHOICE_ANSWER;
        }
    }
}