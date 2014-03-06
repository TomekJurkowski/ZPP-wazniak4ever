
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

        public MultipleChoiceAnswer(int taskId, List<string> choiceList, List<bool> answerList)
            : base(taskId)
        {
            ChoiceList = choiceList;
            AnswerList = answerList;
        }

        public MultipleChoiceAnswer()
        {

        }

        public override string Overview()
        {
            return "MultipleChoiceAnswer with " + ChoiceList.Count + " options.";
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append("\tChoice\tAnswer").AppendLine().AppendLine();
            for (var i = 0; i < ChoiceList.Count; ++i)
            {
                builder.Append(i).Append(")\t").Append(ChoiceList[i])
                    .Append("\t").Append(AnswerList[i]).AppendLine();
            }

            return builder.ToString();
        }

        public override string className()
        {
            return MULTIPLE_CHOICE_ANSWER;
        }
    }
}