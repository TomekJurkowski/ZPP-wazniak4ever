
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WazniakWebsite.DAL;

namespace WazniakWebsite.Models
{
    public class MultipleChoiceAnswer : Answer
    {
        public virtual ICollection<MultiChoice> MultiChoices { get; set; }

        public override string Overview()
        {
            var db = new SchoolContext();
            return "Multiple Choice Answer with " + db.MultiChoices.Count(m => m.MultipleChoiceAnswerID == TaskID) + " options.";
        }

        public override string ToString()
        {
            var db = new SchoolContext();
            var myMultiChoices = db.MultiChoices.Where(m => m.MultipleChoiceAnswerID == TaskID);

            var builder = new StringBuilder();

            builder.Append("There are ").Append(myMultiChoices.Count()).Append(" options to choose from:");
            foreach (var multiChoice in myMultiChoices)
            {
                builder.Append(" [ ").Append(multiChoice).Append(" ];");
            }
            
            return builder.ToString();
        }

        public override string className()
        {
            return MULTIPLE_CHOICE_ANSWER;
        }
    }
}