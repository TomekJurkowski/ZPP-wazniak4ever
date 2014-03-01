using System.ComponentModel.DataAnnotations;

namespace WazniakWebsite.Models
{
    public class TextAnswer : Answer
    {
        [Required]
        public string Text { get; set; }

        public TextAnswer(int taskId, string text) : base(taskId)
        {
            Text = text;
        }

        public TextAnswer()
        {

        }

        public override string Overview()
        {
            if (Text.Length > 100)
            {
                return Text.Substring(0, 99) + "...";                
            }
            return Text;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}