using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WazniakWebsite.Models
{
    public class Answer
    {
        // CONSTANTS representing what kind of Answer is the Task in relationship with
        public const string NO_ANSWER = "no_ans";
        public const string SINGLE_VALUE_ANSWER = "single_val_ans";
        public const string TEXT_ANSWER = "text_ans";
        public const string SINGLE_CHOICE_ANSWER = "single_cho_ans";
        public const string MULTIPLE_CHOICE_ANSWER = "multi_cho_ans";

        [Key]
        [ForeignKey("Task")]
        public int TaskID { get; set; }

        public virtual Task Task { get; set; }

        public virtual string Overview()
        {
            return "Answer associated with task with id=" + TaskID + ".";
        }

        public virtual string className()
        {
            return NO_ANSWER;
        }
    }
}