
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WazniakWebsite.Models
{
    public class MultipleChoiceAnswer : Answer
    {
        public virtual ICollection<MultiChoice> MultiChoices { get; set; }
    
        public MultipleChoiceAnswer(int taskId) : base(taskId)
        {

        }

        public MultipleChoiceAnswer()
        {

        }

        public override string Overview()
        {
            return "MultipleChoiceAnswer with " + MultiChoices.Count + " options.";
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append("\tChoice\tAnswer").AppendLine().AppendLine();
            var i = 0;
            foreach (var multiChoice in MultiChoices)
            {
                builder.Append(++i).Append(")\t").Append(multiChoice).AppendLine();
            }
            
            return builder.ToString();
        }

        public override string className()
        {
            return MULTIPLE_CHOICE_ANSWER;
        }
    }
}