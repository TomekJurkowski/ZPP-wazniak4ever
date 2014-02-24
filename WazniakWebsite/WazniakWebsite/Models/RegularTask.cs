
namespace WazniakWebsite.Models
{
    public class RegularTask : Task
    {
        public string Text { get; set; }

        public override string className()
        {
            return "RegularTask";
        }
    }
}