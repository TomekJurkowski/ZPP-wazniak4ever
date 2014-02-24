using System.ComponentModel.DataAnnotations;

namespace WazniakWebsite.Models
{
    public class MathematicalTask : Task
    {
        public string Text { get; set; }

        public override string className()
        {
            return "MathematicalTask";
        }
    }
}