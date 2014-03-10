
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
            var ret = "1). " + db.MultiChoices.FirstOrDefault(m => m.MultipleChoiceAnswerID == TaskID) + "    2). ...";
            db.Dispose();

            return ret;
        }

        public override string ToString()
        {
            var db = new SchoolContext();
            var myMultiChoices = db.MultiChoices.Where(m => m.MultipleChoiceAnswerID == TaskID);

            var builder = new StringBuilder();

            builder.Append("There are ").Append(myMultiChoices.Count()).Append(" options that could be right or wrong:");
            var i = 0;
            foreach (var multiChoice in myMultiChoices)
            {
                builder.AppendLine().Append(++i).Append(").\t").Append(multiChoice);
            }

            db.Dispose();

            return builder.ToString();
        }

        public override string className()
        {
            return MULTIPLE_CHOICE_ANSWER;
        }
    }
}