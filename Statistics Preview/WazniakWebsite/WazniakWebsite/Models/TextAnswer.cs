using System.ComponentModel.DataAnnotations;

namespace WazniakWebsite.Models
{
    public class TextAnswer : Answer
    {
        [Required]
        public string Text { get; set; }

        public TextAnswer(string text)
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
            return "'" + Text + "'";
        }

        public override string ToString()
        {
            return Text;
        }

        public override string className()
        {
            return TEXT_ANSWER;
        }
    }
}