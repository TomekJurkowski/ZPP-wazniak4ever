
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WazniakWebsite.Models
{
    public class MultipleChoiceAnswer : Answer
    {
        [Required]
        public List<string> ChoiceList { get; set; }

        [Required]
        public List<string> AnswerList { get; set; }

        public override string Overview()
        {
            return "MultipleChoiceAnswer with " + ChoiceList.Count + " options.";
        }
    }
}