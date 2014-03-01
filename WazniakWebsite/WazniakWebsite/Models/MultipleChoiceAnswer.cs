
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WazniakWebsite.Models
{
    public class MultipleChoiceAnswer : Answer
    {
        [Required]
        public List<string> ChoiceList { get; set; }

        [Required]
        public List<bool> AnswerList { get; set; }

        public override string Overview()
        {
            return "MultipleChoiceAnswer with " + ChoiceList.Count + " options.";
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append("\tChoice\tAnswer").AppendLine().AppendLine();
            for (int i = 0; i < ChoiceList.Count; ++i)
            {
                builder.Append(i).Append(")\t").Append(ChoiceList[i])
                    .Append("\t").Append(AnswerList[i]).AppendLine();
            }

            return builder.ToString();
        }
    }
}